using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Orbit.Core.Exceptions;
using Orbit.Models.DTOs;
using Orbit.Models.OrbitDB;
using System.Data;

namespace Orbit.Core.DataAccess
{
	public class ItemCodeService : IItemCodeService
	{
		private readonly ISqlClient sqlClient;
		private readonly IUserSession userSession;
		private readonly string itemCodeSegmentTableName;
		private readonly string itemCodeMappingTableName;

		public ItemCodeService(
			ILogger<ItemCodeService> logger,
			ISqlClient sqlClient,
			IUserSession userSession)
		{
			this.sqlClient = sqlClient;
			this.userSession = userSession;
			this.itemCodeSegmentTableName = this.sqlClient.TableNameMapper(typeof(ItemCodeSegment));
			this.itemCodeMappingTableName = this.sqlClient.TableNameMapper(typeof(ItemCodeMapping));
		}

		public async Task<ItemCodeSegment> AddNewSegmentAsync(ItemCodeSegment segment)
		{
			var maxId = await this.sqlClient.GetMaxID<ItemCodeSegment>();
			segment.ID = (maxId ?? 0) + 1;
			await this.sqlClient.InsertAsync(segment);
			return segment;
		}

		public async Task DeleteMappingsAsync(List<ItemCodeMapping> mappings)
		{
			await this.sqlClient.DeleteAsync(mappings);
		}

		public async Task DeleteSegmentAsync(int id)
		{
			await this.sqlClient.DeleteAsync<ItemCodeSegment>(id);
		}

		public async Task<SegmentDetail> GetItemCodeSegmentDetails(int id)
		{
			var segDetailQuery = $"SELECT * FROM {itemCodeSegmentTableName} WHERE ID = {id}";
			SegmentDetail? detail = (await this.sqlClient.GetData<SegmentDetail>(segDetailQuery)).FirstOrDefault();
			if (detail != null)
			{
				var existingCodesSql = $"SELECT * FROM tblItemCodes WHERE SegmentID = {id}";
				detail.Codes = await this.sqlClient.GetData<ItemCode>(existingCodesSql);

				var segChildDetailQuery = $"SELECT * FROM {itemCodeSegmentTableName} WHERE ParentID = {id}";
				SegmentDetail? child = (await this.sqlClient.GetData<SegmentDetail>(segChildDetailQuery)).FirstOrDefault();
				if (child != null)
				{
					var existingChildCodesSql = $"SELECT c.*  FROM tblItemCodeMappings (NOLOCK) m INNER join tblItemCodes (NOLOCK) p on p.ID = m.ParentID INNER join tblItemCodes (NOLOCK) c on c.ID = m.ChildID  WHERE p.SegmentID = {id} ";
					child.Codes = await this.sqlClient.GetData<ItemCode>(existingChildCodesSql);
					detail.ChildSegment = child;
				}

				if (detail.ParentID != null)
				{
					var segparentDetailQuery = $"SELECT * FROM {itemCodeSegmentTableName} WHERE ID = {detail.ParentID}";
					SegmentDetail? parent = (await this.sqlClient.GetData<SegmentDetail>(segparentDetailQuery)).FirstOrDefault();
					if (parent != null)
					{
						detail.ParentSegment = parent;
					}
				}

				var existingMappingSql = $"SELECT m.*  FROM {this.itemCodeMappingTableName} (NOLOCK) m INNER join tblItemCodes (NOLOCK) p on p.ID = m.ParentID INNER join tblItemCodes (NOLOCK) c on c.ID = m.ChildID  WHERE p.SegmentID = {id}";
				detail.Mappings = await this.sqlClient.GetData<ItemCodeMapping>(existingMappingSql);
			}

			return detail;
		}

		public Task<List<ItemCodeSegment>> GetSegmentsAsync(int orgId)
		{
			var sql = $"SELECT * FROM {this.itemCodeSegmentTableName} WHERE OrganizationID = {orgId} ORDER BY Sequence";
			return this.sqlClient.GetData<ItemCodeSegment>(sql);
		}

		public async Task<ItemCodeSegment> UpdateSegmentAsync(ItemCodeSegment segment)
		{
			await this.sqlClient.UpdateAsync(segment);
			return segment;
		}

		public async Task<List<string>> ValidateAndAddMappings(DataTable table, int parentId)
		{
			var newMappings = new List<ItemCodeMapping>();
			var orgId = userSession.Organization?.ID ?? throw new BadRequestException("Unable to determine your organization");
			var listOfErrors = new List<string>();
			var segsSql = $"SELECT * FROM {this.itemCodeSegmentTableName} WHERE ID = {parentId} OR ParentID = {parentId}";
			var segs = await this.sqlClient.GetData<ItemCodeSegment>(segsSql);
			ItemCodeSegment? parentSeg = segs.Find(x => x.ID == parentId);
			ItemCodeSegment? childSeg = segs.Find(x => x.ParentID == parentId);
			if (parentSeg == null)
			{
				throw new BadRequestException("Unable to get code segment detail for parent segment");
			}

			if (childSeg == null)
			{
				throw new BadRequestException("Unable to get code segment detail for child segment");
			}

			var existingParentCodesSql = $"SELECT * FROM tblItemCodes WHERE SegmentID = {parentSeg.ID}";
			var existingChildCodesSql = $"SELECT * FROM tblItemCodes WHERE SegmentID = {childSeg.ID}";

			var existingParentCodes = await this.sqlClient.GetData<ItemCode>(existingParentCodesSql);
			var existingChildCodes = await this.sqlClient.GetData<ItemCode>(existingChildCodesSql);

			var existingMappingSql = $"SELECT m.*  FROM {this.itemCodeMappingTableName} (NOLOCK) m INNER join tblItemCodes (NOLOCK) p on p.ID = m.ParentID INNER join tblItemCodes (NOLOCK) c on c.ID = m.ChildID  WHERE p.SegmentID = {parentSeg.ID} AND c.SegmentID = {childSeg.ID}";
			var existingMappings = await this.sqlClient.GetData<ItemCodeMapping>(existingMappingSql);

			bool validateCodesOnly = false;
			if (table.Columns.Count == 2)
			{
				validateCodesOnly = true;
			}
			else if (table.Columns.Count != 4)
			{
				listOfErrors.Add(@"Uploaded Excel doesn't have right number of columns required. Supported template are: 
Option 1: Excel with 2 columns namely ParentCode,ChildCode
Option 2: Excel with 2 columns namely ParentCode,ParentDescription,ChildCode,ChildDescription");

				return listOfErrors;
			}

			var newParentCodes = new List<ItemCode>();
			var newChildCodes = new List<ItemCode>();
			var maxItemCodeId = (await this.sqlClient.GetMaxID<ItemCode>()) ?? 0;

			for (int i = 0; i < table.Rows.Count; i++)
			{

				DataRow row = table.Rows[i];
				if (row[0] == null)
				{
					listOfErrors.Add($"Parent code is missing on row number {i + 1}");
					continue;
				}

				if ((validateCodesOnly && (row[1] == null || string.IsNullOrEmpty(row[1].ToString()))) ||
					(!validateCodesOnly && (row[2] == null || string.IsNullOrEmpty(row[2].ToString()))))
				{
					listOfErrors.Add($"Child code is missing on row number {i + 1}");
					continue;
				}

				var parentCodeStr = row[0].ToString();
				var parentCodeNameStr = !validateCodesOnly ? row[1].ToString() : null;
				var childCodeStr = validateCodesOnly ? row[1].ToString() : row[2].ToString();
				var childCodeNameStr = !validateCodesOnly ? row[3].ToString() : null;

				var parentCode = existingParentCodes.Find(x => !string.IsNullOrEmpty(parentCodeStr) && x.Code.Equals(parentCodeStr, StringComparison.InvariantCultureIgnoreCase)) ??
					newParentCodes.Find(x => !string.IsNullOrEmpty(parentCodeStr) && x.Code.Equals(parentCodeStr, StringComparison.InvariantCultureIgnoreCase));
				var childCode = existingChildCodes.Find(x => !string.IsNullOrEmpty(childCodeStr) && x.Code.Equals(childCodeStr, StringComparison.InvariantCultureIgnoreCase)) ??
					newChildCodes.Find(x => !string.IsNullOrEmpty(childCodeStr) && x.Code.Equals(childCodeStr, StringComparison.InvariantCultureIgnoreCase));
				if (parentCode == null)
				{
					if (validateCodesOnly)
					{
						listOfErrors.Add($"Unable to add new parent code to master from row {i + 1} as name/description is missing for parent code");
						continue;
					}
					else
					{
						if (parentCodeNameStr == null)
						{
							listOfErrors.Add($"Parent code description is missing on row number {i + 1}");
							continue;
						}

						parentCode = new ItemCode
						{
							ID = ++maxItemCodeId,
							Code = parentCodeStr,
							SegmentID = parentSeg.ID,
							Name = parentCodeNameStr
						};

						newParentCodes.Add(parentCode);
					}
				}

				if (childCode == null)
				{
					if (validateCodesOnly)
					{
						listOfErrors.Add($"Unable to add new child code to master from row {i + 1} as name/description is missing for child code");
						continue;
					}
					else
					{
						if (childCodeNameStr == null)
						{
							listOfErrors.Add($"Child code description is missing on row number {i + 1}");
							continue;
						}

						childCode = new ItemCode
						{
							ID = ++maxItemCodeId,
							Code = childCodeStr,
							SegmentID = childSeg.ID,
							Name = childCodeNameStr
						};

						newChildCodes.Add(childCode);
					}
				}

				var codeMapping = existingMappings.Find(x => x.ParentID == parentCode.ID && x.ChildID == childCode.ID) ??
					newMappings.Find(x => x.ParentID == parentCode.ID && x.ChildID == childCode.ID);
				if (codeMapping == null)
				{
					codeMapping = new ItemCodeMapping
					{
						ParentID = parentCode.ID,
						ChildID = childCode.ID,
					};

					newMappings.Add(codeMapping);
				}


			}

			var newCodes = new List<ItemCode>();
			newCodes.AddRange(newParentCodes);
			newCodes.AddRange(newChildCodes);

			if (listOfErrors.Count > 0 ||
				(newCodes.Count == 0 && newMappings.Count == 0))
			{
				return listOfErrors;
			}

			using var trans = this.sqlClient.GetTransaction();
			try
			{
				var dateTime = DateTime.Now;
				if (newCodes.Count > 0)
				{
					foreach (var rec in newCodes)
					{
						this.userSession.SetCreatedAuditColumns(rec, null, dateTime);
						this.userSession.SetUpdatedAuditColumns(rec, dateTime);
					}

					await this.sqlClient.InsertAsync(newCodes);
				}

				if (newMappings.Count > 0)
				{
					foreach (var rec in newMappings)
					{
						this.userSession.SetCreatedAuditColumns(rec, null, dateTime);
						this.userSession.SetUpdatedAuditColumns(rec, dateTime);
					}

					await this.sqlClient.InsertAsync(newMappings);
				}
				await trans.CommitAsync();
			}
			catch (Exception)
			{
				await trans.RollbackAsync();
				throw;
			}
			finally
			{
				await trans.DisposeAsync();
			}

			return listOfErrors;
		}
	}
}

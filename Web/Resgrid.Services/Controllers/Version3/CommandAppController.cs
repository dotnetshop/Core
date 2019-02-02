﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http.Cors;
using System.Web.Mvc;
using Resgrid.Model;
using Resgrid.Model.Helpers;
using Resgrid.Model.Providers;
using Resgrid.Model.Services;
using Resgrid.Web.Services.Controllers.Version3.Models.Calls;
using Resgrid.Web.Services.Controllers.Version3.Models.CoreData;
using Resgrid.Web.Services.Controllers.Version3.Models.Groups;
using Resgrid.Web.Services.Controllers.Version3.Models.Personnel;
using Resgrid.Web.Services.Controllers.Version3.Models.Roles;
using Resgrid.Web.Services.Controllers.Version3.Models.Security;
using Resgrid.Web.Services.Controllers.Version3.Models.UnitApp;
using UnitInfoResult = Resgrid.Web.Services.Controllers.Version3.Models.Units.UnitInfoResult;
using System.Net.Http;
using Resgrid.Web.Services.Controllers.Version3.Models.Units;
using Resgrid.Web.Services.Helpers;

namespace Resgrid.Web.Services.Controllers.Version3
{
	[System.Web.Http.Description.ApiExplorerSettings(IgnoreApi = true)]
	[EnableCors(origins: "*", headers: "*", methods: "GET,POST,PUT,DELETE,OPTIONS")]
	public class CommandAppController : V3AuthenticatedApiControllerbase
	{
		private readonly IUsersService _usersService;
		private readonly IDepartmentsService _departmentsService;
		private readonly IUserProfileService _userProfileService;
		private readonly IUnitsService _unitsService;
		private readonly ICallsService _callsService;
		private readonly IDepartmentGroupsService _departmentGroupsService;
		private readonly IPersonnelRolesService _personnelRolesService;
		private readonly ICustomStateService _customStateService;
		private readonly IGeoLocationProvider _geoLocationProvider;
		private readonly IIncidentService _incidentService;

		public CommandAppController(
			IUsersService usersService,
			IDepartmentsService departmentsService,
			IUserProfileService userProfileService,
			IUnitsService unitsService,
			ICallsService callsService,
			IDepartmentGroupsService departmentGroupsService,
			IPersonnelRolesService personnelRolesService,
			ICustomStateService customStateService,
			IGeoLocationProvider geoLocationProvider,
			IIncidentService incidentService
			)
		{
			_usersService = usersService;
			_departmentsService = departmentsService;
			_userProfileService = userProfileService;
			_unitsService = unitsService;
			_callsService = callsService;
			_departmentGroupsService = departmentGroupsService;
			_personnelRolesService = personnelRolesService;
			_customStateService = customStateService;
			_geoLocationProvider = geoLocationProvider;
			_incidentService = incidentService;
		}

		[AcceptVerbs("GET")]
		public UnitAppPayloadResult GetCommandAppCoreData()
		{
			var results = new UnitAppPayloadResult();
			results.Personnel = new List<PersonnelInfoResult>();
			results.Groups = new List<GroupInfoResult>();
			results.Units = new List<UnitInfoResult>();
			results.Roles = new List<RoleInfoResult>();
			results.Statuses = new List<CustomStatusesResult>();
			results.Calls = new List<CallResult>();
			results.UnitStatuses = new List<UnitStatusCoreResult>();
			results.UnitRoles = new List<UnitRoleResult>();

			var users = _departmentsService.GetAllUsersForDepartment(DepartmentId);
			var groups = _departmentGroupsService.GetAllDepartmentGroupsForDepartment(DepartmentId);
			var rolesForUsersInDepartment = _personnelRolesService.GetAllRolesForUsersInDepartment(DepartmentId);
			var allRoles = _personnelRolesService.GetRolesForDepartment(DepartmentId);
			var allProfiles = _userProfileService.GetAllProfilesForDepartment(DepartmentId);
			var allGroups = _departmentGroupsService.GetAllGroupsForDepartment(DepartmentId);
			var units = _unitsService.GetUnitsForDepartment(DepartmentId);
			var unitTypes = _unitsService.GetUnitTypesForDepartment(DepartmentId);


			foreach (var user in users)
			{
				//var profile = _userProfileService.GetProfileByUserId(user.UserId);
				//var group = _departmentGroupsService.GetGroupForUser(user.UserId);

				UserProfile profile = null;
				if (allProfiles.ContainsKey(user.UserId))
					profile = allProfiles[user.UserId];

				DepartmentGroup group = null;
				if (groups.ContainsKey(user.UserId))
					group = groups[user.UserId];

				//var roles = _personnelRolesService.GetRolesForUser(user.UserId);

				List<PersonnelRole> roles = null;
				if (rolesForUsersInDepartment.ContainsKey(user.UserId))
					roles = rolesForUsersInDepartment[user.UserId];

				var result = new PersonnelInfoResult();

				if (profile != null)
				{
					result.Fnm = profile.FirstName;
					result.Lnm = profile.LastName;
					result.Id = profile.IdentificationNumber;
					result.Mnu = profile.MobileNumber;
				}
				else
				{
					result.Fnm = "Unknown";
					result.Lnm = "Check Profile";
					result.Id = "";
					result.Mnu = "";
				}

				result.Eml = user.Email;
				result.Did = DepartmentId;
				result.Uid = user.UserId.ToString();

				if (group != null)
				{
					result.Gid = group.DepartmentGroupId;
					result.Gnm = group.Name;
				}

				result.Roles = new List<string>();
				if (roles != null && roles.Count > 0)
				{
					foreach (var role in roles)
					{
						if (role != null)
							result.Roles.Add(role.Name);
					}
				}

				results.Personnel.Add(result);
			}


			results.Rights = new DepartmentRightsResult();
			var currentUser = _usersService.GetUserByName(UserName);

			if (currentUser == null)
				throw HttpStatusCode.Unauthorized.AsException();

			var department = _departmentsService.GetDepartmentById(DepartmentId, false);

			results.Rights.Adm = department.IsUserAnAdmin(currentUser.UserId);
			results.Rights.Grps = new List<GroupRight>();

			var currentGroup = _departmentGroupsService.GetGroupForUser(currentUser.UserId, DepartmentId);

			if (currentGroup != null)
			{
				var groupRight = new GroupRight();
				groupRight.Gid = currentGroup.DepartmentGroupId;
				groupRight.Adm = currentGroup.IsUserGroupAdmin(currentUser.UserId);

				results.Rights.Grps.Add(groupRight);
			}

			foreach (var group in allGroups)
			{
				var groupInfo = new GroupInfoResult();
				groupInfo.Gid = group.DepartmentGroupId;
				groupInfo.Nme = group.Name;

				if (group.Type.HasValue)
					groupInfo.Typ = group.Type.Value;

				if (group.Address != null)
					groupInfo.Add = group.Address.FormatAddress();

				results.Groups.Add(groupInfo);
			}

			foreach (var unit in units)
			{
				var unitResult = new UnitInfoResult();
				unitResult.Uid = unit.UnitId;
				unitResult.Did = DepartmentId;
				unitResult.Nme = unit.Name;
				unitResult.Typ = unit.Type;

				if (!string.IsNullOrWhiteSpace(unit.Type))
				{
					var unitType = unitTypes.FirstOrDefault(x => x.Type == unit.Type);

					if (unitType != null)
						unitResult.Cid = unitType.CustomStatesId.GetValueOrDefault();
				}
				else
				{
					unitResult.Cid = 0;
				}

				if (unit.StationGroup != null)
				{
					unitResult.Sid = unit.StationGroup.DepartmentGroupId;
					unitResult.Snm = unit.StationGroup.Name;
				}

				results.Units.Add(unitResult);

				// Add unit roles for this unit
				var roles = _unitsService.GetRolesForUnit(unit.UnitId);
				foreach (var role in roles)
				{
					var roleResult = new UnitRoleResult();
					roleResult.Name = role.Name;
					roleResult.UnitId = role.UnitId;
					roleResult.UnitRoleId = role.UnitRoleId;

					results.UnitRoles.Add(roleResult);
				}
			}

			var unitStatuses = _unitsService.GetAllLatestStatusForUnitsByDepartmentId(DepartmentId);
			
			foreach (var us in unitStatuses)
			{
				var unitStatus = new UnitStatusCoreResult();
				unitStatus.UnitId = us.UnitId;
				unitStatus.StateType = (UnitStateTypes)us.State;
				unitStatus.StateTypeId = us.State;
				unitStatus.Type = us.Unit.Type;
				unitStatus.Timestamp = us.Timestamp.TimeConverter(department);
				unitStatus.Name = us.Unit.Name;

				if (us.DestinationId.HasValue)
					unitStatus.DestinationId = us.DestinationId.Value;

				results.UnitStatuses.Add(unitStatus);
			}

			foreach (var role in allRoles)
			{
				var roleResult = new RoleInfoResult();
				roleResult.Rid = role.PersonnelRoleId;
				roleResult.Nme = role.Name;

				results.Roles.Add(roleResult);
			}

			var customStates = _customStateService.GetAllActiveCustomStatesForDepartment(DepartmentId);

			foreach (var customState in customStates)
			{
				if (customState.IsDeleted)
					continue;

				foreach (var stateDetail in customState.GetActiveDetails())
				{
					if (stateDetail.IsDeleted)
						continue;

					var customStateResult = new CustomStatusesResult();
					customStateResult.Id = stateDetail.CustomStateDetailId;
					customStateResult.Type = customState.Type;
					customStateResult.StateId = stateDetail.CustomStateId;
					customStateResult.Text = stateDetail.ButtonText;
					customStateResult.BColor = stateDetail.ButtonColor;
					customStateResult.Color = stateDetail.TextColor;
					customStateResult.Gps = stateDetail.GpsRequired;
					customStateResult.Note = stateDetail.NoteType;
					customStateResult.Detail = stateDetail.DetailType;

					results.Statuses.Add(customStateResult);
				}

			}

			var calls = _callsService.GetActiveCallsByDepartment(DepartmentId).OrderByDescending(x => x.LoggedOn);

			if (calls != null && calls.Any())
			{
				foreach (var c in calls)
				{
					var call = new CallResult();

					call.Cid = c.CallId;
					call.Pri = c.Priority;
					call.Ctl = c.IsCritical;
					call.Nme = c.Name;
					call.Noc = c.NatureOfCall;
					call.Map = c.MapPage;
					call.Not = c.Notes;

					if (String.IsNullOrWhiteSpace(c.Address) && !String.IsNullOrWhiteSpace(c.GeoLocationData))
					{
						var geo = c.GeoLocationData.Split(char.Parse(","));

						if (geo.Length == 2)
							call.Add = _geoLocationProvider.GetAddressFromLatLong(double.Parse(geo[0]), double.Parse(geo[1]));
					}
					else
						call.Add = c.Address;

					call.Add = c.Address;
					call.Geo = c.GeoLocationData;
					call.Lon = c.LoggedOn.TimeConverter(department);
					call.Ste = c.State;
					call.Num = c.Number;

					results.Calls.Add(call);
				}
			}
			else
			{
				// This is a hack due to a bug in the current units app! -SJ 1-31-2016
				var call = new CallResult();
				call.Cid = 0;
				call.Pri = 0;
				call.Ctl = false;
				call.Nme = "No Call";
				call.Noc = "";
				call.Map = "";
				call.Not = "";
				call.Add = "";
				call.Geo = "";
				call.Lon = DateTime.UtcNow;
				call.Ste = 0;
				call.Num = "";

				results.Calls.Add(call);
			}


			return results;
		}

		public HttpResponseMessage Options()
		{
			var response = new HttpResponseMessage();
			response.StatusCode = HttpStatusCode.OK;
			response.Headers.Add("Access-Control-Allow-Origin", "*");
			response.Headers.Add("Access-Control-Request-Headers", "*");
			response.Headers.Add("Access-Control-Allow-Methods", "GET,POST,PUT,DELETE,OPTIONS");

			return response;
		}
	}
}

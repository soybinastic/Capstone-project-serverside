using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConstructionMaterialOrderingApi.Repositories;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace ConstructionMaterialOrderingApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly IHardwareStoreUserRepository _hardwareStoreUserRepository;
        private readonly INotificationRepository _notificationRepository;
        private readonly IWareHouseAdminRepository _wareHouseAdminRepository;
        private readonly IStoreAdminRepository _storeAdminRepository;

        public NotificationController(IHardwareStoreUserRepository hardwareStoreUserRepository, 
            INotificationRepository notificationRepository,
            IWareHouseAdminRepository wareHouseAdminRepository,
            IStoreAdminRepository storeAdminRepository)
        {
            _hardwareStoreUserRepository = hardwareStoreUserRepository;
            _notificationRepository = notificationRepository;
            _wareHouseAdminRepository = wareHouseAdminRepository;
            _storeAdminRepository = storeAdminRepository;
        } 

        [HttpGet]
        [Route("get-warehouse-notif-number")]
        [Authorize(Roles = "WarehouseAdmin")]
        public async Task<IActionResult> GetWarehouseNotificationNumber()
        {
            var userAppId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var warehouseAdmin = await _wareHouseAdminRepository.GetWarehouseAdminByAccountId(userAppId);
            var notificationNumber = await _notificationRepository.GetWarehouseNotificationNumber(warehouseAdmin.WarehouseId);
            return Ok(ConvertToJson(notificationNumber));
        }

        [HttpGet]
        [Route("get-warehouse-notifs")]
        [Authorize(Roles = "WarehouseAdmin")]
        public async Task<IActionResult> GetWarehouseNotification()
        {
            var userAppId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var warehouseAdmin = await _wareHouseAdminRepository.GetWarehouseAdminByAccountId(userAppId);
            var notifications = await _notificationRepository.GetWarehouseNotifications(warehouseAdmin.WarehouseId);
            return Ok(ConvertToJson(notifications));
        }

        //TODO : implenting branch notification system.
        [HttpGet]
        [Route("get-branch-notif-number")]
        [Authorize(Roles = "StoreAdmin")]
        public async Task<IActionResult> GetBranchNotificationNumber()
        {
            var userAppId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var storeAdmin = await _storeAdminRepository.GetStoreAdminByAccountId(userAppId);
            var notificationNumber = await _notificationRepository.GetNotificationNumber(storeAdmin.BranchId);
            return Ok(ConvertToJson(notificationNumber));
        }

        [HttpGet]
        [Route("get-branch-notifs")]
        [Authorize(Roles = "StoreAdmin")]
        public async Task<IActionResult> GetBranchNotification()
        {
            var userAppId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var storeAdmin = await _storeAdminRepository.GetStoreAdminByAccountId(userAppId);
            var notifications = await _notificationRepository.GetNotifications(storeAdmin.BranchId);
            return Ok(ConvertToJson(notifications));
        }

        private string ConvertToJson<T>(T obj)
        {
            var jsonObj = JsonConvert.SerializeObject(obj, new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });

            return jsonObj;
        }
    }
}

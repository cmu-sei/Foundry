/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

using AutoMapper;
using Microsoft.Extensions.Logging;
using Stack.Http.Exceptions;
using Stack.Http.Identity;
using Stack.Patterns.Service;
using Stack.Patterns.Service.Models;
using Foundry.Portal.Data;
using Foundry.Portal.Data.Entities;
using Foundry.Portal.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Foundry.Portal.Services
{
    public class SettingService : Service<Setting>
    {
        ISettingRepository _settingRepository;

        public SettingService(ISettingRepository settingRepository, CoreOptions options, IStackIdentityResolver userResolver, ILoggerFactory loggerFactory, IMapper mapper)
            : base(options, userResolver, loggerFactory, mapper)
        {
            _settingRepository = settingRepository ?? throw new ArgumentNullException("settingRepository");
        }

        public async Task<SettingDetail> GetByKey(string key)
        {
            return Map<SettingDetail>(await _settingRepository.GetByKey(key));
        }

        public async Task<SettingDetail> Add(SettingCreate model)
        {
            if (!Identity.Permissions.Contains(SystemPermissions.Administrator))
                throw new EntityPermissionException("Requires elevated permissions.");

            if (_settingRepository.DbContext.Settings.Any(s => s.Key.ToLower() == model.Key.ToLower()))
                throw new EntityDuplicateException("Key '" + model.Key + "' already exists");

            var setting = new Setting
            {
                Key = model.Key,
                Value = model.Value
            };

            var result = await _settingRepository.Add(setting);
            return Map<SettingDetail>(result);
        }

        public async Task<SettingDetail> Update(SettingUpdate model)
        {
            if (!Identity.Permissions.Contains(SystemPermissions.Administrator))
                throw new EntityPermissionException("Requires elevated permissions.");

            var setting = await _settingRepository.GetByKey(model.Key);

            if (setting == null)
                throw new EntityNotFoundException("Setting was not found.");

            setting.Key = model.Key;
            setting.Value = model.Value;

            var result = await _settingRepository.Update(setting);
            return Map<SettingDetail>(result);
        }

        public async Task<PagedResult<Setting, SettingDetail>> GetAll(SettingDataFilter search)
        {
            return await PagedResult<Setting, SettingDetail>(_settingRepository.GetAll(), search);
        }
    }
}


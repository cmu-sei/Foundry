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
using Foundry.Communications.Data;
using Foundry.Communications.Data.Entities;
using Foundry.Communications.Data.Repositories;
using Foundry.Communications.ViewModels;
using Stack.Http.Identity;
using Stack.Patterns.Service;
using Stack.Patterns.Service.Models;
using System.Linq;
using System.Threading.Tasks;

namespace Foundry.Communications.Services
{
    public class MessageService : StackIdentityRepositoryService<CommunicationDbContext, IMessageRepository, Message>
    {
        public MessageService(IStackIdentityResolver stackIdentityResolver, IMessageRepository messageRepository, IMapper mapper)
            : base(stackIdentityResolver, messageRepository, mapper) { }

        public async Task<PagedResult<Message, MessageSummary>> GetAll(MessageDataFilter filter = null)
        {
            var globalId = Identity.Id;

            var query = Repository.GetAll()
                .Where(m => m.Recipients.Any(r => r.TargetId == globalId));

            return await PagedResultFactory.Execute<Message, MessageSummary>(query, filter, Identity);
        }
    }
}

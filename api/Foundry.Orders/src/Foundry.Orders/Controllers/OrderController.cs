/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

using AppMailClient;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Stack.Http.Identity;
using Stack.Http.Options;
using Stack.Http.Identity.Attributes;
using Foundry.Orders.Data.Entities;
using Foundry.Orders.Options;
using Foundry.Orders.ViewModels;
using Stack.Patterns.Service.Models;
using System;
using System.Threading.Tasks;

namespace Foundry.Orders.Controllers
{
    /// <summary>
    /// order controller
    /// </summary>
    [SecurityHeaders]
    [StackAuthorize]
    public class OrderController : BaseController
    {
        Services.OrderService OrderService { get; }
        IAppMailClient AppMailClient { get; }
        BrandingOptions BrandingOptions { get; }
        MessageOptions MessageOptions { get; }

        /// <summary>
        /// creates an instance of order controller
        /// </summary>
        /// <param name="identityResolver"></param>
        /// <param name="mapper"></param>
        /// <param name="orderService"></param>
        /// <param name="appMailClient"></param>
        /// <param name="displayOptions"></param>
        /// <param name="messageLinkOptions"></param>
        public OrderController(IStackIdentityResolver identityResolver, IMapper mapper, Services.OrderService orderService, IAppMailClient appMailClient, BrandingOptions displayOptions, MessageOptions messageLinkOptions)
            : base(identityResolver, mapper)
        {
            OrderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
            AppMailClient = appMailClient ?? throw new ArgumentNullException(nameof(appMailClient));
            BrandingOptions = displayOptions ?? throw new ArgumentNullException(nameof(displayOptions));
            MessageOptions = messageLinkOptions ?? throw new ArgumentNullException(nameof(messageLinkOptions));
        }

        /// <summary>
        /// get all orders
        /// </summary>
        /// <param name="dataFilter"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/orders")]
        [ProducesResponseType(typeof(PagedResult<Order, OrderSummary>), 200)]
        public async Task<IActionResult> GetAll([FromQuery]OrderDataFilter dataFilter = null)
        {
            return Ok(await OrderService.GetAll(dataFilter));
        }

        /// <summary>
        /// get order by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/order/{id}")]
        [ProducesResponseType(typeof(OrderDetail), 200)]
        public async Task<IActionResult> GetById([FromRoute]int id)
        {
            return Ok(await OrderService.GetById(id));
        }

        /// <summary>
        /// get order for edit by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/order/{id}/edit")]
        [ProducesResponseType(typeof(OrderEdit), 200)]
        public async Task<IActionResult> GetByIdForEdit([FromRoute]int id)
        {
            return Ok(Mapper.Map<OrderEdit>(await OrderService.OrderRepository.GetById(id)));
        }

        /// <summary>
        /// add new order
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/orders")]
        [ProducesResponseType(typeof(OrderDetail), 201)]
        public async Task<IActionResult> Add([FromBody]OrderEdit model)
        {
            var result = await OrderService.Add(model);
            return Created("api/order/" + result.Id, result);
        }

        /// <summary>
        /// update order
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("api/order/{id}")]
        [ProducesResponseType(typeof(OrderDetail), 200)]
        public async Task<IActionResult> Update([FromRoute]int id, [FromBody]OrderEdit model)
        {
            return Ok(await OrderService.Update(id, model));
        }

        /// <summary>
        /// delete order
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("api/order/{id}")]
        [ProducesResponseType(typeof(bool), 200)]
        public async Task<IActionResult> Delete([FromRoute]int id)
        {
            await OrderService.Delete(id);
            return Ok(true);
        }

        /// <summary>
        /// update the status of an order
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("api/order/{id}/status")]
        [ProducesResponseType(typeof(OrderDetail), 200)]
        public async Task<IActionResult> SetStatus([FromRoute]int id, [FromBody]string model)
        {
            var status = (OrderStatus)Enum.Parse(typeof(OrderStatus), model);
            return Ok(await OrderService.SetStatus(id, status));
        }

        /// <summary>
        /// send email for order
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/order/{id}/email")]
        [ProducesResponseType(typeof(OrderDetail), 200)]
        public async Task<IActionResult> SendEmail([FromRoute]int id)
        {
            string link = string.Format(MessageOptions.LinkFormat, id);
            string text = string.Format(MessageOptions.TextFormat, link);
            string subject = string.Format(MessageOptions.SubjectFormat, BrandingOptions.ApplicationName);

            var message = new MailMessage
            {
                Subject = subject,
                Text = text
            };

            return Ok(await AppMailClient.Send(message));
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult Error()
        {
            return View();
        }
    }
}


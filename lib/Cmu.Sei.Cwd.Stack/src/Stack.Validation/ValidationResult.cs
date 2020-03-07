/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Collections.Generic;
using System.Linq;

namespace Stack.Validation
{
    /// <summary>
    /// validation result object to be returned by the api
    /// </summary>
    public class ValidationResult
    {
        /// <summary>
        /// the validation message
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// the collection of validation errors
        /// </summary>
        public IEnumerable<ValidationError> Errors { get; } = new List<ValidationError>();

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="modelState"></param>
        public ValidationResult(ModelStateDictionary modelState)
        {
            Message = "Validation Failed";
            Errors = modelState.Keys.SelectMany(key => modelState[key].Errors
                .Select(x => new ValidationError(key, x.ErrorMessage)));
        }
    }
}


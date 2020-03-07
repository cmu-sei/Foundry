/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

using Stack.Validation.Attributes;
using Stack.Validation.Exceptions;
using Stack.Validation.Rules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Stack.Validation.Handlers
{
    /// <summary>
    /// processes all validation rules where applied to the given ViewModel
    /// </summary>
    public abstract class ValidationHandler
    {
        public abstract object GetService(Type type);

        public IEnumerable<IValidationRule<TViewModel>> GetValidationRulesFor<TViewModel>()
            where TViewModel : class
        {
            var rules = new List<IValidationRule<TViewModel>>();

            var validation = typeof(TViewModel).GetAttribute<ValidationAttribute>();

            if (validation != null)
            {
                foreach (var rule in validation.RuleTypes)
                {
                    var validationRule = GetValidationRuleFor<TViewModel>(rule);

                    rules.Add(validationRule);
                }
            }

            return rules;
        }

        IValidationRule<TViewModel> GetValidationRuleFor<TViewModel>(Type validationRuleType)
            where TViewModel : class
        {
            if (validationRuleType == null)
                throw new ValidationHandlerException("Validation Rule Type is null.");

            var constructor = validationRuleType.GetConstructors().SingleOrDefault();

            if (constructor == null)
                throw new ValidationHandlerException($"Could not determine constructor for {validationRuleType.Name}.");

            var parameters = constructor.GetParameters().ToList();

            var objects = new List<object>();

            parameters.ForEach(p => objects.Add(GetService(p.ParameterType)));

            var validationRule = (IValidationRule<TViewModel>)constructor.Invoke(objects.ToArray());

            if (validationRule == null)
                throw new ValidationHandlerException($"Could not invoke constructor for {validationRuleType.Name}.");

            return validationRule;
        }

        public async Task ValidateRulesFor<TViewModel>(TViewModel model)
             where TViewModel : class
        {
            var validation = model.GetType().GetAttribute<ValidationAttribute>();

            if (validation == null)
                return;

            var rules = GetValidationRulesFor<TViewModel>();

            foreach (var rule in rules)
                await ValidateRule(rule, model);
        }

        public async Task ValidateRule<TValidationRule, TViewModel>(TViewModel viewModel)
           where TValidationRule : class, IValidationRule<TViewModel>
           where TViewModel : class
        {
            var validationRule = GetValidationRuleFor<TViewModel>(typeof(TValidationRule));
            await ValidateRule<IValidationRule<TViewModel>, TViewModel>(validationRule, viewModel);
        }

        public async Task ValidateRule<TViewModel>(IValidationRule<TViewModel> validationRule, TViewModel viewModel)
            where TViewModel : class
        {
            await ValidateRule<IValidationRule<TViewModel>, TViewModel>(validationRule, viewModel);
        }

        public async Task ValidateRule<TViewModel>(Type rule, TViewModel viewModel)
            where TViewModel : class
        {
            var validationRule = (IValidationRule<TViewModel>)Activator.CreateInstance(rule);
            await ValidateRule<IValidationRule<TViewModel>, TViewModel>(validationRule, viewModel);
        }

        public async Task ValidateRule<TValidationRule, TViewModel>(IValidationRule<TViewModel> validationRule, TViewModel viewModel)
            where TValidationRule : class, IValidationRule<TViewModel>
            where TViewModel : class
        {
            if (validationRule != null)
            {
                await validationRule.Validate(viewModel);
            }
        }
    }
}

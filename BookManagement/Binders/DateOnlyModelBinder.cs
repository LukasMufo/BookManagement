using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using System.Globalization;

namespace BookManagement.Binders
{
    /// <summary>
    /// model binder for binding DateOnly objects.
    /// </summary>
    public class DateOnlyModelBinder : IModelBinder
    {
        /// <summary>
        /// Asynchronously binds the model using the provided binding context.
        /// </summary>
        /// <param name="bindingContext">The binding context.</param>
        /// <returns>A task of the async operation.</returns>
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
            {
                throw new ArgumentNullException(nameof(bindingContext));
            }

            var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            if (valueProviderResult == ValueProviderResult.None)
            {
                return Task.CompletedTask;
            }

            bindingContext.ModelState.SetModelValue(bindingContext.ModelName, valueProviderResult);

            var value = valueProviderResult.FirstValue;
            if (string.IsNullOrEmpty(value))
            {
                return Task.CompletedTask;
            }

            if (DateTime.TryParseExact(value, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
            {
                bindingContext.Result = ModelBindingResult.Success(new DateOnly(date.Year, date.Month, date.Day));
            }
            else
            {
                bindingContext.ModelState.TryAddModelError(bindingContext.ModelName, "Invalid date format");
            }

            return Task.CompletedTask;
        }
    }
    /// <summary>
    /// Model binder provider for DateOnly objects.
    /// </summary>
    public class DateOnlyModelBinderProvider : IModelBinderProvider
    {
        /// <summary>
        /// Get the binder for DateOnly
        /// </summary>
        /// <param name="context">The model binder provider context.</param>
        /// <returns>A model binder for the DateOnly, or null if not found.</returns>
        public IModelBinder? GetBinder(ModelBinderProviderContext context)
        {
            if (context.Metadata.ModelType == typeof(DateOnly))
            {
                return new BinderTypeModelBinder(typeof(DateOnlyModelBinder));
            }

            return null;
        }
    }
}

using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using System.Globalization;

namespace BookManagement.Binders
{
    public class DateOnlyModelBinder : IModelBinder
    {
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

    public class DateOnlyModelBinderProvider : IModelBinderProvider
    {
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

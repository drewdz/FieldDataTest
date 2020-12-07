using FieldDataTest.ViewModels;
using System.Windows.Forms;

namespace FieldDataTest.Bindings
{
    public static class BaseFormExtensions
    {
        public static void Bind<TViewModel>(this BaseForm<TViewModel> form, Binding binding) where TViewModel : IViewModel, new()
        {
            form.Bindings.Add(binding.ViewModelProperty, binding);
        }
    }
}

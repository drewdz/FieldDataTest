using System.Windows.Forms;

namespace FieldDataTest.Bindings
{
    public class BindingBuilder
    {
        #region Properties

        public object Target { get; set; }

        public string ViewProperty { get; set; }

        public string ViewModelProperty { get; set; }

        #endregion Properties

        #region Operations

        public BindingBuilder For(string property)
        {
            ViewProperty = property;
            return this;
        }

        public BindingBuilder To(string property)
        {
            ViewModelProperty = property;
            return this;
        }

        public static BindingBuilder Create()
        {
            return new BindingBuilder();
        }

        public static BindingBuilder BindOn(object target)
        {
            return Create().On(target);
        }

        public BindingBuilder On(object target)
        {
            Target = target;
            return this;
        }

        public Binding<TView, TViewModel> Build<TView, TViewModel>() where TViewModel : class
        {
            return new Binding<TView, TViewModel>(Target)
            {
                ViewProperty = ViewProperty,
                ViewModelProperty = ViewModelProperty
            };
        }

        #endregion Operations
    }
}

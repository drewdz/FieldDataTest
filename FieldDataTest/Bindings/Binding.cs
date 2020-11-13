using System;
using System.Reflection;

namespace FieldDataTest.Bindings
{
    public abstract class Binding
    {
        #region Constructors

        public Binding(object child)
        {
            Target = child;
        }

        #endregion Constructors

        #region Operations

        public abstract void PropertyChanged();

        #endregion Operations

        #region Properties

        public object Target { get; set; }

        public string ViewProperty { get; set; }

        public string ViewModelProperty { get; set; }

        #endregion Properties

        #region Operations

        public void Apply()
        {
            GC.SuppressFinalize(this);
        }

        #endregion Operations
    }

    public class Binding<TView, TViewModel> : Binding
        where TViewModel : class
    {
        #region Constructors

        public Binding(object child)
            : base(child)
        {
        }

        #endregion Constructors

        #region Operations

        public Binding<TView, TViewModel> For(string property)
        {
            ViewProperty = property;
            return this;
        }

        public Binding<TView, TViewModel> To(string property)
        {
            ViewModelProperty = property;
            return this;
        }

        public override void PropertyChanged()
        {
        }

        #endregion Operations
    }
}

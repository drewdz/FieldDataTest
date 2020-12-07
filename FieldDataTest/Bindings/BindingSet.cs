using FieldDataTest.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FieldDataTest.Bindings
{
    public class BindingSet<TView, TViewModel>
        where TView : BaseForm<TViewModel>
        where TViewModel : IViewModel, new()
    {
        #region Fields

        private readonly TView _View;
        private readonly TViewModel _ViewModel;
        private List<Binding> _Bindings = new List<Binding>();

        #endregion Fields

        #region Constructors

        public BindingSet() { }

        private BindingSet(TView v, TViewModel vm)
        {
            _View = v;
            _ViewModel = vm;
        }

        #endregion Constructors

        #region Helpers

        public static BindingSet<TView, TViewModel> Create(TView v, TViewModel vm)
        {
            return new BindingSet<TView, TViewModel>(v, vm);
        }

        #endregion Helpers

        #region Operations

        public Binding<V, VM> Bind<V, VM>(object child)
            where VM : class
        {
            return new Binding<V, VM>(child);
        }

        public void Apply(IBindable set)
        {
            set.Bindings = new Dictionary<string, Binding>();
            foreach (var binding in _Bindings)
            {
                set.Bindings.Add(binding.ViewModelProperty, binding);
                binding.Apply();
            }
        }

        #endregion Operations
    }
}

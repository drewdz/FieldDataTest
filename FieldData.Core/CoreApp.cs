using FieldData.Core.ViewModels;
using MvvmCross.IoC;
using MvvmCross.ViewModels;

namespace FieldData.Core
{
    public class CoreApp : MvxApplication
    {
        public override void Initialize()
        {
            CreatableTypes()
                .EndingWith("Service")
                .AsInterfaces()
                .RegisterAsLazySingleton();

            RegisterAppStart<MainViewModel>();
        }
    }
}

using DataFactory.Generators;
using DataFactory.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataFactory
{
    public sealed class GeneratorFactory
    {
        #region Fields

        private List<IGenerator> _Generators;
        private static GeneratorFactory _Instance = null;

        #endregion Fields

        #region Constructors

        private GeneratorFactory()
        {
            _Generators = new List<IGenerator>();
            var assembly = this.GetType().Assembly;
            foreach (var type in assembly.GetTypes())
            {
                if (!type.GetInterfaces().Contains(typeof(IGenerator))) continue;
                //  get the constructor
                var c = type.GetConstructor(new Type[] { });
                if (c == null) continue;
                var o = c.Invoke(new object[] { }) as IGenerator;
                if (o == null) continue;
                _Generators.Add(o);
            }
        }

        #endregion Constructors

        #region Operations

        public static IGenerator Create(FieldActivity activity)
        {
            _Instance = _Instance ?? new GeneratorFactory();
            return _Instance.GetGenerator(activity);
        }

        public IGenerator GetGenerator(FieldActivity activity)
        {
            var generator = _Generators.FirstOrDefault(g => g.Name.Equals(activity.Type));
            if (generator == null) throw new Exception($"Could not find generator for {activity.Type}");
            generator.Init(activity);
            return generator;
        }

        #endregion Operations
    }
}

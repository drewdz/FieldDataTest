﻿using DataFactory.Model;
using System;
using System.Collections.Generic;

namespace DataFactory.Generators
{
    public interface IGenerator
    {
        #region Properties

        string Name { get; }

        string ActionName { get; }

        #endregion Properties

        #region Operations

        void Init(FieldActivity activity);

        void SetQueues();

        List<EventData> Generate(DateTimeOffset startDate, int sampleCount);

        List<EventData> Generate(long millis);

        #endregion Operations
    }
}

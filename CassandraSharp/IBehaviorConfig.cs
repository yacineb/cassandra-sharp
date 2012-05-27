﻿// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
// http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace CassandraSharp
{
    using Apache.Cassandra;

    public interface IBehaviorConfig
    {
        string KeySpace { get; }

        ConsistencyLevel? ReadConsistencyLevel { get; }

        int? TTL { get; }

        ConsistencyLevel? WriteConsistencyLevel { get; }

        int? MaxRetries { get; }

        bool? RetryOnNotFound { get; }

        bool? RetryOnTimeout { get; }

        bool? RetryOnUnavailable { get; }

        int? SleepBeforeRetry { get; }
    }
}
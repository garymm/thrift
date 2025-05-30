// Licensed to the Apache Software Foundation(ASF) under one
// or more contributor license agreements.See the NOTICE file
// distributed with this work for additional information
// regarding copyright ownership.The ASF licenses this file
// to you under the Apache License, Version 2.0 (the
// "License"); you may not use this file except in compliance
// with the License. You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing,
// software distributed under the License is distributed on an
// "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
// KIND, either express or implied. See the License for the
// specific language governing permissions and limitations
// under the License.

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Thrift5253;

namespace Thrift.Compile.net9.Impl.Thrift5253
{
    class MyServiceImpl : MyService.IAsync
    {
        public Task<AsyncProcessor> AsyncProcessor_(AsyncProcessor? input, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(new AsyncProcessor() { Foo = input?.Foo ?? 0 });
        }

        public Task<BrokenResult> Broken(BrokenArgs? input, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(new BrokenResult() { Foo = input?.Foo ?? 0 });
        }

        public Task<Client> Client_(Client? input, CancellationToken cancellationToken = default)
        {
            _ = cancellationToken;
            return Task.FromResult(new Client() { Foo = input?.Foo ?? 0 });
        }

        public Task<IAsync> IAsync_(IAsync? input, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(new IAsync() { Foo = input?.Foo ?? 0 });
        }

        public Task<InternalStructs> InternalStructs_(InternalStructs? input, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(new InternalStructs() { Foo = input?.Foo ?? 0 });
        }

        public Task TestAsync(CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public Task TestXsync(CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public Task<WorksRslt> Works(WorksArrrgs? input, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(new WorksRslt() { Foo = input?.Foo ?? 0 });
        }
    }
}

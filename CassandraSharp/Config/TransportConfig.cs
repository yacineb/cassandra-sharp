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

namespace CassandraSharp.Config
{
    using System.Xml.Serialization;

    public class TransportConfig
    {
        public TransportConfig()
        {
            Port = 9160;
            Type = "Framed";
            Recoverable = true;
        }

        [XmlAttribute("port")]
        public int Port { get; set; }

        [XmlAttribute("type")]
        public string Type { get; set; }

        [XmlAttribute("timeout")]
        public int Timeout { get; set; }

        [XmlAttribute("poolSize")]
        public int PoolSize { get; set; }

        [XmlAttribute("recoverable")]
        public bool Recoverable { get; set; }

        [XmlAttribute("user")]
        public string User { get; set; }

        [XmlAttribute("password")]
        public string Password { get; set; }

        [XmlAttribute("cqlver")]
        public string CqlVersion { get; set; }
    }
}
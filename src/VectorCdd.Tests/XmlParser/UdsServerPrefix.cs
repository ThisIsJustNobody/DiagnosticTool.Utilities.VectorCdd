using DiagnosticTool.Utilities.VectorCdd.Common;

using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace DiagnosticTool.Utilities.VectorCdd.XmlParser
{
    /// <summary>
    /// UDS 服务前缀
    /// </summary>
    public class UdsServerPrefix(byte? id = null, string? name = null, byte[]? data = null)
        : INotifyPropertyChanged, IEnumerable<UdsServerPrefix>
    {
        #region 静态

        /// <summary>
        /// 默认的 UDS 服务前缀
        /// </summary>
        public static UdsServerPrefix DefaultUdsServerPrefix => new(null, "所有服务")
    {
        new UdsServerPrefix(null, "自定义"),
        new UdsServerPrefix(0x10, "诊断会话控制")
        {
            new(0x01, "默认会话"),
            new(0x02, "编程会话"),
            new(0x03, "扩展会话"),
            new(0x40, "整车制造商"),
            new(0x60, "系统供应商"),
        },
        new UdsServerPrefix(0x11, "ECU 重置")
        {
            new(0x01, "硬重置"),
            new(0x02, "点火钥匙关闭/重置"),
            new(0x03, "软重置"),
            new(0x04, "启动快速断电"),
            new(0x05, "禁用快速断电")
        },
        new UdsServerPrefix(0x14, "清除 DTC 信息") { new(null, "清除所有 DTC", [0xFF, 0xFF, 0xFF]), },
        new UdsServerPrefix(0x19, "读取 DTC 信息")
        {
            new(0x01, "按状态掩码读 DTC 的数量")
            {
                new UdsServerPrefix(null, "读取所有 DTC", [0xFF]),
                new UdsServerPrefix(null, "读取所有当前 DTC", [0x01]),
                new UdsServerPrefix(null, "读取所有历史 DTC", [0x08]),
                new UdsServerPrefix(null, "读取当前和历史 DTC", [0x09]),
            },
            new(0x02, "按状态掩码读 DTC")
            {
                new UdsServerPrefix(null, "读取所有 DTC", [0xFF]),
                new UdsServerPrefix(null, "读取所有当前 DTC", [0x01]),
                new UdsServerPrefix(null, "读取所有历史 DTC", [0x08]),
                new UdsServerPrefix(null, "读取当前和历史 DTC", [0x09]),
            },
            new(0x0A, "读取支持的 DTC"),
            new(0x17, "读取用户定义内存 DTC")
            {
                new UdsServerPrefix(null, "读取所有 DTC", [0xFF]),
                new UdsServerPrefix(null, "读取所有当前 DTC", [0x01]),
                new UdsServerPrefix(null, "读取所有历史 DTC", [0x08]),
                new UdsServerPrefix(null, "读取当前和历史 DTC", [0x09]),
            },
        },
        new UdsServerPrefix(0x22, "按标识符读取数据"),
        new UdsServerPrefix(0x23, "按地址读取内容"),
        new UdsServerPrefix(0x27, "安全访问")
        {
            new(0x01, "请求 Seed(0x01)"),
            new(0x02, "发送 Key(0x02)"),
            new(0x03, "请求 Seed(0x03)"),
            new(0x04, "发送 Key(0x04)"),
            new(0x05, "请求 Seed(0x05)"),
            new(0x06, "发送 Key(0x06)"),
            new(0x07, "请求 Seed(0x07)"),
            new(0x08, "发送 Key(0x08)"),
            new(0x61, "请求系统供应商 Seed(0x61)"),
            new(0x62, "发送系统供应商 Key(0x62)"),
        },
        new UdsServerPrefix(0x28, "通讯控制")
        {
            new(0x00, "启用 Rx 和 Tx") { new(0x01, "所有与应用程序相关的通信"), new(0x02, "所有与网络管理相关的通信"), new(0x03, "所有网络管理和应用程序相关的通信")},
            new(0x01, "启用 Rx 并禁用 Tx") { new(0x01, "所有与应用程序相关的通信"), new(0x02, "所有与网络管理相关的通信"), new(0x03, "所有网络管理和应用程序相关的通信")},
            new(0x02, "禁用 Rx 并启用 Tx") { new(0x01, "所有与应用程序相关的通信"), new(0x02, "所有与网络管理相关的通信"), new(0x03, "所有网络管理和应用程序相关的通信")},
            new(0x03, "禁用 Rx 和 Tx") { new(0x01, "所有与应用程序相关的通信"), new(0x02, "所有与网络管理相关的通信"), new(0x03, "所有网络管理和应用程序相关的通信")},
        },
        new UdsServerPrefix(0x2A, "按周期性标识符读取数据"),
        new UdsServerPrefix(0x2C, "动态定义数据标识符"),
        new UdsServerPrefix(0x2E, "按标识符写入数据"),
        new UdsServerPrefix(0x2F, "按标识符的输入输出控制"),
        new UdsServerPrefix(0x31, "例程控制") { new(0x01, "启动例程"), new(0x02, "停止例程"), new(0x03, "请求例程结果") },
        new UdsServerPrefix(0x34, "请求下载"),
        new UdsServerPrefix(0x35, "请求上传"),
        new UdsServerPrefix(0x36, "传输数据"),
        new UdsServerPrefix(0x37, "请求退出"),
        new UdsServerPrefix(0x3D, "按地址写内存"),
        new UdsServerPrefix(0x3E, "会话保持") { new(null, "会话保持", [0x00]), new(null, "会话保持（无响应）", [0x80]), },
        new UdsServerPrefix(0x83, "访问计时参数"),
        new UdsServerPrefix(0x84, "受保护的数据传输"),
        new UdsServerPrefix(0x85, "控制 DTC 设置") { new(0x01, "打开 DTC 记录"), new(0x02, "关闭 DTC 记录") },
        new UdsServerPrefix(0x87, "链路控制"),
    };

        #endregion 静态

        #region 字段、属性

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private byte? _id = id;

        /// <summary>
        /// 服务 Id
        /// </summary>
        [JsonInclude]
        public byte? Id
        {
            get => _id;
            set
            {
                if (_id != value)
                {
                    _id = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(IdHexString));
                    OnPropertyChanged(nameof(Description));
                }
            }
        }

        /// <summary>
        /// 服务 Id（十六进制字符串）
        /// </summary>
        [JsonPropertyName("IdHexString")]
        public string? IdHexString
        {
            get => Id?.ToString("X2");
            set => Id = value == null ? null : Convert.ToByte(value, 16);
        }

        private string _name = name ?? "New Item";

        /// <summary>
        /// 服务名称
        /// </summary>
        [JsonPropertyName("name")]
        public string Name
        {
            get => _name;
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(Description));
                }
            }
        }

        private byte[]? _data = data;

        /// <summary>
        /// 数据
        /// </summary>
        public byte[]? Data
        {
            get => _data;
            set
            {
                if (_data != value)
                {
                    _data = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(DataHexString));
                    OnPropertyChanged(nameof(Description));
                }
            }
        }

        /// <summary>
        /// 数据字符串
        /// </summary>
        [JsonPropertyName("DataHexString")]
        public string? DataHexString
        {
            get => Data == null ? null : BitConverter.ToString(Data).Replace("-", " ");
            set =>
                Data =
                    value == null
                        ? null
                        : Convert.FromHexString(value.Replace("-", "").Replace(" ", ""));
        }

        private UdsServerPrefix? _parent;

        /// <summary>
        /// 上层对象
        /// </summary>
        public UdsServerPrefix? Parent
        {
            get => _parent;
            set
            {
                if (_parent != value)
                {
                    _parent = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 子对象
        /// </summary>
        [JsonPropertyName("Children")]
        public ObservableCollection<UdsServerPrefix> Children { get; set; } = [];

        /// <summary>
        /// 子对象列表
        /// </summary>
        public List<UdsServerPrefix> ChildrenList
        {
            get => [.. Children];
            set
            {
                Children.Clear();
                value.ForEach(Children.Add);
            }
        }

        /// <summary>
        /// 按照服务 Id 获取子服务
        /// </summary>
        public UdsServerPrefix? this[byte i] => Children.FirstOrDefault(x => x.Id == i);

        /// <summary>
        /// 前缀数据
        /// </summary>
        public byte[] PrefixData
        {
            get
            {
                var d = new List<byte>();
                if (Id.HasValue)
                    d.Add(Id.Value);
                if (Data != null)
                    d.AddRange(Data);
                return [.. d];
            }
        }

        /// <summary>
        /// 完整前缀数据
        /// </summary>
        public byte[] FullPrefixData => Parent == null ? PrefixData : [.. Parent.FullPrefixData, .. PrefixData];

        /// <summary>
        /// 描述
        /// </summary>
        public string Description
        {
            get
            {
                var p1 = Id != null ? $"({Id.Value:X2}) " : "";
                var p2 = Data != null ? DataHexString + " " : "";
                if (string.IsNullOrEmpty(p1) && string.IsNullOrEmpty(p2))
                {
                    return Name;
                }
                else
                {
                    return $"{p1}{p2}- {Name}";
                }
            }
        }

        /// <summary>
        /// 转换方法
        /// </summary>
        public IConvert? Converter { get; set; }
        #endregion 字段、属性

        public void Add(UdsServerPrefix child)
        {
            child.Parent = this;
            Children.Add(child);
        }

        public override string ToString()
        {
            return Description;
        }

        #region IEnumerable<UdsServerPrefix> 接口
        IEnumerator<UdsServerPrefix> IEnumerable<UdsServerPrefix>.GetEnumerator()
        {
            return ((IEnumerable<UdsServerPrefix>)Children).GetEnumerator();
        }

        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable)Children).GetEnumerator();
        }
        #endregion IEnumerable<UdsServerPrefix> 接口
    }
}

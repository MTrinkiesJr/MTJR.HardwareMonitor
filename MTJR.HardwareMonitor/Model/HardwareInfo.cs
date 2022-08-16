using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using MTJR.HardwareMonitor.Extensions;

namespace MTJR.HardwareMonitor.Model
{
    /// <summary>
    /// Model to reflect received data from Open Hardware Monitor 
    /// </summary>
    public class HardwareInfo
    {

        private string _serverId;

        /// <summary>
        /// The specified id of this <see cref="HardwareInfo"/>
        /// </summary>
        [JsonPropertyName("id")]
        public int Id { get; set; }

        /// <summary>
        /// The name like text
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Some <see cref="Text"/> names are different to defaults, this property equalizes the name
        /// At the moment only captured on core temperatures
        /// eg. Some processors expose 'CPU Package' which is the default
        /// Some others expose like 'Core #1 - #8'
        /// </summary>
        public string ResolvedText => Text.ToLower() == "sensor" ? "Computer" : Text.StartsWith("Core") && Text.Contains("-") ? "CPU Package" : Text.Replace("#", "");
        /// <summary>
        /// Child <see cref="HardwareInfo"/> 
        /// </summary>
        public List<HardwareInfo> Children { get; set; }

        /// <summary>
        /// Resolved complete path 
        /// </summary>
        [JsonIgnore]
        public string CompleteName => ResolveName();


        /// <summary>
        /// Extracted unit from the value
        /// </summary>
        [JsonIgnore]
        public string ValueUnit => string.IsNullOrEmpty(Value) ? "n/a" : Value.Split(' ').Length > 1 ? Value.Split(' ')[1] : "n/a";

        /// <summary>
        /// Minimum value
        /// </summary>
        public string Min { get; set; }

        /// <summary>
        /// Parsed <see cref="Min"/> value as <see cref="double"/>
        /// </summary>
        [JsonIgnore]
        public double ParsedMin => ParseValue(Min);

        /// <summary>
        /// The current value
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Parsed <see cref="Value"/> as <see cref="double"/>
        /// </summary>

        [JsonIgnore] public double ParsedValue => ParseValue(Value);

        /// <summary>
        /// Maximum value
        /// </summary>
        public string Max { get; set; }

        /// <summary>
        /// Parsed <see cref="Max"/> as <see cref="double"/>
        /// </summary>
        [JsonIgnore]
        public double ParsedMax => ParseValue(Max);

        /// <summary>
        /// URL to displayed image (referenced from open Hardware Monitor)
        /// </summary>
        public string ImageUrl { get; set; }

        /// <summary>
        /// The resolved parent <see cref="HardwareInfo"/>
        /// </summary>
        [JsonIgnore]
        public HardwareInfo Parent { get; set; }

        /// <summary>
        /// Resolved <see cref="HardwareType"/> by <see cref="ImageUrl"/>
        /// Some <see cref="ImageUrl"/> of value 'HardwareType.Transparent/> inherit the <see cref="HardwareType"/> from <see cref="Parent"/> 
        /// </summary>
        [JsonIgnore] public HardwareType HardwareType => ImageUrl.GetHardwareType(Parent);

        /// <summary>
        /// Resolved <see cref="HardwareType"/> by <see cref="ImageUrl"/>
        /// Some <see cref="ImageUrl"/> of value 'HardwareType.Transparent/> inherit the <see cref="HardwareType"/> from <see cref="Parent"/>
        /// SubType is only set when <see cref="HardwareType"/> is <see cref="HardwareType.Gpu"/> to the correct GPu vendor
        /// </summary>
        [JsonIgnore] public HardwareType SubHardwareType => HardwareType == HardwareType.Gpu ? ResolveSubType() : HardwareType.None;

        /// <summary>
        /// Resolved <see cref="ValueType"/> by <see cref="ImageUrl"/>
        /// Some <see cref="ImageUrl"/> of value 'HardwareType.Transparent/> inherit the <see cref="HardwareType"/> from <see cref="Parent"/> 
        /// </summary>
        [JsonIgnore] public ValueType ValueType => ImageUrl.GetValueType(Parent);

        [JsonIgnore]
        private List<HardwareInfo> _flat = new List<HardwareInfo>();


        /// <summary>
        /// Resolves the parents for each <see cref="Children"/> recursive
        /// </summary>
        public void ResolveParents(string serverId)
        {
            _serverId = serverId;
            foreach (var hardwareInfo in Children)
            {
                hardwareInfo.Parent = this;
                hardwareInfo.ResolveParents(serverId);
            }
        }


        /// <summary>
        /// Resolves the complete path with <see cref="Text"/> recursive
        /// </summary>
        /// <returns><see cref="string"/></returns>
        public string ResolveName()
        {
            var parentName = "";

            if (Parent != null)
            {
                parentName = Parent.ResolveName();
            }

            var separator = string.IsNullOrEmpty(parentName) ? "" : "_";
            return $"{parentName}{separator}{Text}";
        }


        /// <summary>
        /// Parses the retrieved value to <see cref="double"/>
        /// </summary>
        /// <param name="value">The value from <see cref="Value"/>, <see cref="Min"/> or <see cref="Max"/></param>
        /// <returns><see cref="double"/></returns>
        private double ParseValue(string value)
        {
            if (!string.IsNullOrEmpty(Value) && double.TryParse(value.Split(' ')[0], out var result))
            {
                return result;
            }
            return -1;
        }


        /// <summary>
        /// Retrieves a flat list of all <see cref="HardwareInfo"/>
        /// </summary>
        /// <returns><see cref="List{HardwareInfo}"/></returns>
        public List<HardwareInfo> GetChildrenFlat()
        {
            var list = new List<HardwareInfo>();
            list = list.Concat(Children).ToList();

            foreach (var hardwareInfo in Children)
            {
                list = list.Concat(hardwareInfo.GetChildrenFlat()).ToList();
            }

            return list;
        }


        /// <summary>
        /// Retrieves a <see cref="HardwareInfo"/> by its nam and optional <see cref="HardwareInfo"/>
        /// </summary>
        /// <param name="name">The name of the <see cref="HardwareInfo"/></param>
        /// <param name="hardwareType">The <see cref="HardwareType"/> of the <see cref="HardwareInfo"/></param>
        /// <param name="valueType">The <see cref="ValueType"/> of the <see cref="HardwareInfo"/></param>
        /// <returns><see cref="HardwareInfo"/></returns>
        public HardwareInfo FindByName(string name, HardwareType hardwareType = HardwareType.None, ValueType valueType = ValueType.None)
        {
            if (!_flat.Any())
            {
                _flat = GetChildrenFlat();
            }

            if (hardwareType == HardwareType.None)
            {
                return _flat.FirstOrDefault(a => a.Text.ToLower() == name.ToLower());
            }

            return _flat.FirstOrDefault(a => a.Text.ToLower() == name.ToLower() && a.HardwareType == hardwareType && a.ValueType == valueType);
        }


        /// <summary>
        /// Searches for <see cref="HardwareInfo"/> with given parent <see cref="HardwareType"/> and value <see cref="HardwareType"/>
        /// </summary>
        /// <param name="parentType">The <see cref="HardwareType"/> of the <see cref="Parent"/> should be</param>
        /// <param name="valueType">The <see cref="HardwareType"/> the <see cref="HardwareInfo"/> should be</param>
        /// <returns><see cref="HardwareInfo"/></returns>
        public HardwareInfo FindInfoByNestedType(HardwareType parentType, ValueType valueType)
        {
            if (!_flat.Any())
            {
                _flat = GetChildrenFlat();
            }

            var parent = _flat.FirstOrDefault(a => a.HardwareType == parentType);

            if (parent != null)
            {
                HardwareInfo result = null;

                do
                {
                    foreach (var hardwareInfo in parent.Children)
                    {
                        if (hardwareInfo.ValueType == valueType)
                        {
                            result = hardwareInfo;
                            break;
                        }
                    }
                } while (result == null);

                if (string.IsNullOrEmpty(result.Value))
                {
                    return result.Children.FirstOrDefault(a => !string.IsNullOrEmpty(a.Value));
                }
            }

            return null;
        }


        /// <summary>
        /// Retrieves the children of give <see cref="HardwareType"/>
        /// </summary>
        /// <param name="type">the <see cref="HardwareType"/> the children should be</param>
        /// <returns></returns>
        public List<HardwareInfo> GetChildrenOfType(ValueType type)
        {
            return Children.Where(a => a.ValueType == type && !string.IsNullOrEmpty(a.Value)).ToList();
        }


        /// <summary>
        /// Searches for multiple <see cref="HardwareInfo"/> with given parent <see cref="HardwareType"/> and value <see cref="ValueType"/>
        /// </summary>
        /// <param name="parentType">The <see cref="HardwareType"/> of the <see cref="Parent"/> should be</param>
        /// <param name="valueType">The <see cref="ValueType"/> the <see cref="HardwareInfo"/> should be</param>
        /// <returns><see cref="List{HardwareInfo}"/></returns>
        public List<HardwareInfo> FindInfosByNestedType(HardwareType parentType, ValueType valueType)
        {
            if (!_flat.Any())
            {
                _flat = GetChildrenFlat();
            }

            var parents = _flat.Where(a => a.HardwareType == parentType);

            var results = new List<HardwareInfo>();


            foreach (var parent in parents)
            {
                var subParent = parent.Children.FirstOrDefault(a => a.ValueType == valueType);

                if (subParent != null)
                {
                    results.AddRange(subParent.Children);

                    subParent.Children.ForEach(a => results.AddRange(a.GetChildrenOfType(valueType)));
                }
            }

            return results;
        }

        private HardwareType ResolveSubType()
        {
            var imageName = "";
            if (!string.IsNullOrEmpty(ImageUrl))
            {
                imageName = ImageUrl.ToLower().Split('/')[1];
            }
            switch (imageName)
            {
                case "nvidia.png":
                    return HardwareType.Nvidia;
                case "amd.png":
                    return HardwareType.Amd;
                default:
                    return HardwareType.None;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutpostOmega.Game.datums.UserInterface
{
    /// <summary>
    /// Base UI-Object that is able to represent every GWEN control
    /// </summary>
    public class Base
    {
        /// <summary>
        /// Gets or sets the <see cref="Base"/> with the specified name.
        /// </summary>
        public Base this[BaseType BaseType] 
        { 
            get
            {
                return (from child in _Children
                        where child.Type == BaseType
                        select child).SingleOrDefault();
            }
            set
            {
                if (!_Children.Exists(child => child.Type == BaseType))
                    _Children.Add(value);
            }
        }

        /// <summary>
        /// Contains all child elements
        /// </summary>
        public List<Base> Children
        {
            get
            {
                return _Children;
            }
        }

        public void FireEvent(BaseEvent Name, object[] args)
        {
            if (EventFired != null)
                EventFired(Name, args);
        }

        public delegate void EventFiredHandler(BaseEvent Name, object[] args);
        public event EventFiredHandler EventFired;

        public delegate void AttributeChangedHandler(AttributeType Type, object Value);
        public event AttributeChangedHandler AttributeChanged;

        /// <summary>
        /// Gets or sets the <see cref="BaseAttribute"/> with the specified name.
        /// </summary>
        public BaseAttribute this[AttributeType AttributeType] 
        {
            get
            {
                return (from attribute in Attributes
                        where attribute.Type == AttributeType
                        select attribute).SingleOrDefault();
            }
            set
            {
                if (!Attributes.Exists(attrib => attrib.Type == AttributeType))
                    Attributes.Add(value);
                else
                    Attributes.Find(attrib => attrib.Type == AttributeType).Value = value.Value;

                if (AttributeChanged != null)
                    AttributeChanged(AttributeType, value.Value);
            }
        }

        /// <summary>
        /// Name of this <see cref="Base"/>
        /// </summary>
        public BaseType Type 
        { 
            get
            {
                return _Type;
            }
        }
        private BaseType _Type;

        public Base(BaseType Type)
        {
            _Type = Type;
        }

        private List<Base> _Children = new List<Base>();
        public List<BaseAttribute> Attributes = new List<BaseAttribute>();

        public object GetValue(string AttributeType)
        {
            try
            {
                var attrType = (AttributeType)Enum.Parse(typeof(AttributeType), AttributeType, true);
                var hit = this[attrType];
                if (hit != null)
                    return hit.Value;
                else
                    return null;
            }
            catch
            {
                throw new Exception("Could not interpret '" + AttributeType + "'");
            }
        }

        public Base GetChild(string Type)
        {
            Base Child = null;

            BaseType eType;
            if (Enum.TryParse<BaseType>(Type, true, out eType))
            {
                return this[eType];
            }

            return Child;
        }

        public Base GetChild(string Attribute, string Value)
        {
            Base Child = null;

            AttributeType eAttribute;
            if (Enum.TryParse<AttributeType>(Attribute, true, out eAttribute))
            {
                foreach(var child in Children)
                {
                    if (child[eAttribute] != null && child[eAttribute].Value.ToString() == Value)
                        return Child;
                }
            }

            return Child;
        }

        public void SetAttribute(string Attribute, string Value)
        {
            AttributeType eAttribute;
            if (Enum.TryParse<AttributeType>(Attribute, true, out eAttribute))
            {
                if(Attributes.Exists(m=>m.Type == eAttribute))
                {
                    this[eAttribute] = new BaseAttribute() { Type = eAttribute, Value = Value };
                }
                else
                {
                    Attributes.Add(new BaseAttribute() { Type = eAttribute, Value = Value });
                }
            }
        }

        public Base[] GetChildren(string Attribute, string Value)
        {
            List<Base> Children = new List<Base>();
            
            AttributeType eAttribute;
            if (Enum.TryParse<AttributeType>(Attribute, true, out eAttribute))
            {
                foreach (var child in Children)
                {
                    if (child[eAttribute] != null && child[eAttribute].Value.ToString() == Value)
                        Children.Add(child);
                }
            }


            return Children.ToArray();
        }
    }

    /// <summary>
    /// Base Type that is used by a <see cref="Base"/>
    /// </summary>
    public enum BaseType
    {
        @Base,
        Button,
        CheckBox,
        CollapsibleList,
        ColorPicker,
        ColorSlider,
        ComboBox,
        DockBase,
        LabeledCheckBox,
        LabeledRadioButton,
        ListBox,
        Menu,
        MenuSrtip,
        MessageBox,
        MultilineTextBox,
        NumericUpDown,
        ProgressBar,
        Properties,
        RadioButton,
        RadioButtonGroup,
        RichLabel,
        ScrollBar,
        StatusBar,
        TabControl,
        TextBoxNumeric,
        TextBoxPassword,
        TreeControl,
        VerticalSlider,
        HorizontalSlider,
        WindowControl,
        Label,
        VerticalSplitter,
        HorizontalSplitter,
        Panel1,
        Panel2,
        GroupBox,
        TextBox,
        ImagePanel
    }

    /// <summary>
    /// Base Events <see cref="Base"/>
    /// </summary>
    public enum BaseEvent
    {
        Click,
    }

    /// <summary>
    /// Attribute structure that is hold by a <see cref="Base"/>
    /// </summary>
    public class BaseAttribute
    {
        public AttributeType Type { get; set; }
        public object Value { get; set; }
    }

    /// <summary>
    /// Attribute Type that is used by a <see cref="BaseAttribute"/>
    /// </summary>
    public enum AttributeType
    {
        X,
        Y,
        Dock,
        Width,
        Height,
        Padding,
        Margin,
        userdata,
        Disabled,
        Hidden,
        inputenabled,
        Tabable,
        Handle,
        Text,
        Name,
        ImageName
    }
}

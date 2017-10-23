using Emgu.CV;
using Emgu.CV.Structure;
using System;

namespace ImageEvaluatorInterfaces.BaseClasses
{
    public abstract class NamedData
    {
        public string Name { get; }
        public string Description { get; }
        public abstract Type DataType { get; }


        public NamedData(string description, string name)
        {
            Name = name;
            Description = description;
        }


        public LongNamedData AsLongNamedData()
        {
            return this as LongNamedData;
        }

        public BooleanNamedData AsBooleanNamedData()
        {
            return this as BooleanNamedData;
        }

        public DoubleNamedData AsDoubleNamedData()
        {
            return this as DoubleNamedData;
        }

        public FloatNamedData AsFloatNamedData()
        {
            return this as FloatNamedData;
        }

        public DateTimeNamedData AsDateTimeNamedData()
        {
            return this as DateTimeNamedData;
        }

        public TimeSpanNamedData AsTimeSpanNamedData()
        {
            return this as TimeSpanNamedData;
        }

        public StringNamedData AsStringNamedData()
        {
            return this as StringNamedData;
        }

        public EmguByteNamedData AsEmguByteNamedData()
        {
            return this as EmguByteNamedData;
        }

        public EmguFloatNamedData AsEmguFloatNamedData()
        {
            return this as EmguFloatNamedData;
        }



    }


    public class NamedData<T> : NamedData
    {
        T Value { get; set; }


        public override Type DataType
        {
            get { return typeof(T); }
        }

        public NamedData(T value, string name, string description)
            : base(name, description)
        {
            Value = value;
        }


        public static implicit operator T(NamedData<T> data)
        {
            return data.Value;
        }
    }


    public class LongNamedData : NamedData<long>
    {
        public LongNamedData(long value, string description, string name)
            : base(value, description, name)
        {
        }

    }

    public class DoubleNamedData : NamedData<double>
    {
        public DoubleNamedData(double value, string description, string name)
            : base(value, description, name)
        {
        }
    }

    public class FloatNamedData : NamedData<float>
    {
        public FloatNamedData(float value, string description, string name)
            : base(value, description, name)
        {
        }
    }

    public class BooleanNamedData : NamedData<bool>
    {
        public BooleanNamedData(bool value, string description, string name)
            : base(value, description, name)
        {
        }
    }

    public class DateTimeNamedData : NamedData<DateTime>
    {
        public DateTimeNamedData(DateTime value, string description, string name)
            : base(value, description, name)
        {
        }
    }

    public class TimeSpanNamedData : NamedData<TimeSpan>
    {
        public TimeSpanNamedData(TimeSpan value, string description, string name)
            : base(value, description, name)
        {
        }
    }

    public class StringNamedData : NamedData<string>
    {
        public StringNamedData(string value, string description, string name)
            : base(value, description, name)
        {
        }
    }

    public class EmguByteNamedData : NamedData<Image<Gray, byte>[]>
    {
        public EmguByteNamedData(Image<Gray, byte>[] value, string description, string name)
            : base(value, description, name)
        {
        }
    }

    public class EmguFloatNamedData : NamedData<Image<Gray, byte>[]>
    {
        public EmguFloatNamedData(Image<Gray, byte>[] value, string description, string name)
            : base(value, description, name)
        {
        }
    }


}

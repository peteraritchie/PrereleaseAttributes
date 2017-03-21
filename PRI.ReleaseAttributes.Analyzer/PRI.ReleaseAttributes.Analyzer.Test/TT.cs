	public struct Value
	{
		private int _value;
		public static readonly Value MinValue = new Value {_value = 1};
		public static readonly Value MaxValue = new Value {_value = 100};

		public static Value operator +(Value lhs, Value rhs)
		{
			var result = new Value {_value = lhs._value + rhs._value};
			return result;
		}
	}
public class TTT
{
	private readonly object _value = Value.MinValue + Value.MaxValue;
}

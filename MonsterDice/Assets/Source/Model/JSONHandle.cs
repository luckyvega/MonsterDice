using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Model
{
	public abstract class JsonCreationConverter<T> : JsonConverter
	{
		protected abstract T Create(Type objectType, JObject jsonObject);

		public override bool CanConvert(Type objectType)
		{
			return typeof(T).IsAssignableFrom(objectType);
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			var jsonObject = JObject.Load(reader);
			var target = Create(objectType, jsonObject);
			serializer.Populate(jsonObject.CreateReader(), target);
			return target;
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			throw new NotImplementedException();
		}
	}

	public class JsonExpressionConverter : JsonCreationConverter<Expression>
	{
		protected override Expression Create(Type objectType, JObject jsonObject)
		{
			var opType = jsonObject["op"].ToString();
			if (opType == "and" || opType == "or")
				return new Logic();
			else
				return new Comparison();
		}
	}

	public class JsonValueConverter : JsonCreationConverter<Value>
	{
		protected override Value Create(Type objectType, JObject jsonObject)
		{
			if (jsonObject["filterId"] != null)
				return new Aggregate();
			else if (jsonObject["propertyName"] != null)
				return new Property();
			else
				return new Constant();
		}
	}

	public abstract class Value
	{
		public abstract string debugPrint();
	}

	public class Constant : Value
	{
		public string constantValue;

		public override string debugPrint()
		{
			return string.Format("constantValue: {0}", constantValue);
		}
	}

	public class Property : Value
	{
		public string propertyName;

		public override string debugPrint()
		{
			return string.Format("propertyName: {0}", propertyName);
		}
	}

	public class Aggregate : Property
	{
		public int filterId;
		public string aggregateMethod;

		public override string debugPrint()
		{
			return string.Format("filterId: {2}, propertyName: {1}({0})", propertyName, aggregateMethod, filterId);
		}
	}

	public class Condition
	{
		public string target;
		public Filter[] filters;
		public Expression[] criteria;

		public string debugPrint()
		{
			string debugStr = string.Format("target: {0}\nfilters:\n", target);
			foreach (Filter f in filters)
				debugStr += f.debugPrint();
			debugStr += "criteria:\n";
			foreach (Expression e in criteria)
				debugStr += e.debugPrint();
			return debugStr;
		}
	}

	public class Filter
	{
		public int id;
		public Expression expression;

		public string debugPrint()
		{
			return "filterId: " + id + "\n" + expression.debugPrint();
		}
	}

	public abstract class Expression
	{
		public string op;

		public abstract string debugPrint();
	}

	public class Comparison : Expression
	{
		public Value leftValue;
		public Value rightValue;

		public override string debugPrint()
		{
			return leftValue.debugPrint() + string.Format(" {0} ", op) + rightValue.debugPrint() + "\n";
		}
	}

	public class Logic : Expression
	{
		public Expression[] logicElements;

		public override string debugPrint()
		{
			string debugStr = "[\n"+ logicElements[0].debugPrint();
			for (int i = 1; i < logicElements.Length; i++)
			{
				debugStr += op + "\n";
				debugStr += logicElements[i].debugPrint();
			}
			debugStr += "]\n";
			return debugStr;
		}
	}
}


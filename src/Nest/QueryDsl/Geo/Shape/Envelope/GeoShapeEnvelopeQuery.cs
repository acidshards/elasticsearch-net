﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Nest.Resolvers;
using Newtonsoft.Json;
using Elasticsearch.Net;

namespace Nest
{
	[JsonObject(MemberSerialization = MemberSerialization.OptIn)]
	public interface IGeoShapeEnvelopeQuery : IGeoShapeQuery
	{
		[JsonProperty("shape")]
		IEnvelopeGeoShape Shape { get; set; }
	}

	public class GeoShapeEnvelopeQuery : FieldNameQuery, IGeoShapeEnvelopeQuery
	{
		bool IQuery.Conditionless { get { return false; } }
		public IEnvelopeGeoShape Shape { get; set; }

		protected override void WrapInContainer(IQueryContainer container)
		{
			container.GeoShape = this;
		}
	}

	public class GeoShapeEnvelopeQueryDescriptor<T> : IGeoShapeEnvelopeQuery where T : class
	{
		private IGeoShapeEnvelopeQuery Self => this;
		string IQuery.Name { get; set; }
		bool IQuery.Conditionless
		{
			get
			{
				return Self.Field.IsConditionless() || Self.Shape == null || !Self.Shape.Coordinates.HasAny();
			}
		}
		PropertyPathMarker IFieldNameQuery.Field { get; set; }
		IEnvelopeGeoShape IGeoShapeEnvelopeQuery.Shape { get; set; }

		public GeoShapeEnvelopeQueryDescriptor<T> Name(string name)
		{
			Self.Name = name;
			return this;
		}
		
		public GeoShapeEnvelopeQueryDescriptor<T> OnField(string field)
		{
			Self.Field = field;
			return this;
		}

		public GeoShapeEnvelopeQueryDescriptor<T> OnField(Expression<Func<T, object>> objectPath)
		{
			Self.Field = objectPath;
			return this;
		}

		public GeoShapeEnvelopeQueryDescriptor<T> Coordinates(IEnumerable<IEnumerable<double>> coordinates)
		{
			if (Self.Shape == null)
				Self.Shape = new EnvelopeGeoShape();
			Self.Shape.Coordinates = coordinates;
			return this;
		}
	}
}

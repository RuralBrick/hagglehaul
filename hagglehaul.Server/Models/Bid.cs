﻿using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace hagglehaul.Server.Models
{
    public class Bid
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("driverEmail")]
        public string DriverEmail { get; set; } = null!;

        [BsonElement("tripId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string TripId { get; set; } = null!;

        [BsonElement("centsAmount")]
        public uint CentsAmount { get; set; }
    }
}
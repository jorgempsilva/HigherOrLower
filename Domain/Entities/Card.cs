﻿using Domain.Enums;

namespace Domain.Entities
{
    public class Card(CardValue value, CardSuit suit)
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public CardValue Value { get; private set; } = value;
        public CardSuit Suit { get; private set; } = suit;
    }
}

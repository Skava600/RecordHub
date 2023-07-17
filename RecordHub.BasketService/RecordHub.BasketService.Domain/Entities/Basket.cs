﻿namespace RecordHub.BasketService.Domain.Entities
{
    public class Basket
    {
        public string UserName { get; set; }
        public IReadOnlyList<BasketItem> Items => items.AsReadOnly();

        private List<BasketItem> items;
        private bool isDirty = true;
        private double totalPrice;

        public Basket()
        {
        }

        public Basket(string userName)
        {
            UserName = userName;
            items = new List<BasketItem>();
        }

        public double TotalPrice
        {
            get
            {
                if (isDirty)
                {
                    double totalPrice = 0;
                    foreach (var item in Items)
                    {
                        totalPrice += item.Price * item.Quantity;
                    }

                    this.totalPrice = totalPrice;
                    isDirty = false;
                }

                return this.totalPrice;
            }
        }

        public void RemoveItem(BasketItem item)
        {
            items.Remove(item);
            isDirty = true;
        }

        public void UpdateItem(BasketItem item)
        {
            var oldItemIndex = items.FindIndex(i => i.ProductId == item.ProductId);
            if (oldItemIndex == -1)
            {
                items.Add(item);
            }
            else
            {
                items[oldItemIndex] = item;
            }

            isDirty = true;
        }
    }
}

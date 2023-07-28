namespace RecordHub.BasketService.Domain.Entities
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
            items = new List<BasketItem>();
        }

        public Basket(string userName, IEnumerable<BasketItem>? items = default)
        {
            UserName = userName;

            this.items = items == null
                ? new List<BasketItem>()
                : items.ToList();
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

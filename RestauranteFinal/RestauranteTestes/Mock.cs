
namespace RestauranteTestes
{
    internal class Mock<T>
    {
        public object Object { get; internal set; }

        internal object As<T1>()
        {
            throw new NotImplementedException();
        }

        internal object Setup(Func<object, object> value)
        {
            throw new NotImplementedException();
        }
    }
}
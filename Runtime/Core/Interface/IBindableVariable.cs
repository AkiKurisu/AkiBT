namespace Kurisu.AkiBT
{
    /// <summary>
    /// Interface to bind <see cref="T"/> value between different type of variables
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IBindableVariable<T>
    {
        T Value { get; set; }
        /// <summary>
        /// Bind to other bindable variable with type <see cref="T"/> constraint
        /// </summary>
        /// <param name="other"></param>
        void Bind(IBindableVariable<T> other);
    }
}
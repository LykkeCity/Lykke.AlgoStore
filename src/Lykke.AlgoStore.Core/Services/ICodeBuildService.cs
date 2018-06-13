using Lykke.AlgoStore.Core.Validation;

namespace Lykke.AlgoStore.Core.Services
{
    /// <summary>
    /// The starting point of all code validation
    /// </summary>
    public interface ICodeBuildService
    {
        /// <summary>
        /// Begins a new code validation session given a piece of code
        /// </summary>
        /// <param name="code">The code to validate</param>
        /// <returns>An instance of <see cref="ICodeBuildSession"/></returns>
        /// <exception cref="System.ArgumentNullException">Thrown when <paramref name="code"/> is null or empty</exception>
        ICodeBuildSession StartSession(string code);
    }
}

using System;
using System.Linq;
using UnityEngine.InputSystem;

namespace Joa.Extensions
{
    public static class Extensions
    {
        // Note 1: for demo purposes, since I only have a keyboard at my disposal, this only looks for keyboard bindings
        // Note 2: Implementing composite bindings also requires a little extra work, which will be left for the students to figure out
        // Hint: We can check if a binding is composite or part of a composite bind like in the extension method below
        public static int GetFirstBindingIndex(this InputAction action)
            => action.bindings // Starting IEnumerable (ReadOnlyArray)
                .Select((binding, index) => (b: binding, i: index)) // Create a new IEnumerable<Tuple<InputBinding>, int> for bindings and their indices
                .Where(tuple => tuple.b.IsKeyboardBinding() && !tuple.b.IsCompositeBinding()) // Create a new IEnumerable consisting of objects matching the condition
                .Select(tuple => tuple.i) // Create a new IEnumerable<int> for indices of the objects
                .DefaultIfEmpty(-1) // If no value matched our search, set first value to -1
                .First(); // Get the first result

        private static bool IsKeyboardBinding(this InputBinding binding)
            => string.IsNullOrEmpty(binding.effectivePath)
                ? binding.path.StartsWith("<Keyboard>/", StringComparison.Ordinal) // This is used to define the culture of the comparison
                : binding.effectivePath.StartsWith("<Keyboard>/", StringComparison.Ordinal); // Ordinal is the most stable option. Defaults to CurrentCulture

        private static bool IsCompositeBinding(this InputBinding binding)
            => binding.isComposite || binding.isPartOfComposite;
    }
}

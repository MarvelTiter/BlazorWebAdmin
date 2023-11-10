namespace Project.AppCore.Routers
{
    public struct MenuGroup : IEquatable<MenuGroup>
    {
        public string Name { get; set; }
        public string Icon { get; set; }

        public readonly bool Equals(MenuGroup other)
        {
            return Name == other.Name && Icon == other.Icon;
        }

        public override readonly bool Equals(object? obj)
        {
            return obj is MenuGroup group && Equals(group);
        }

        public override readonly int GetHashCode()
        {
            return $"{Name}{Icon}".GetHashCode();
        }

        public static bool operator ==(MenuGroup left, MenuGroup right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(MenuGroup left, MenuGroup right)
        {
            return !(left == right);
        }
    }
}

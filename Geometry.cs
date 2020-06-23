using Microsoft.Xna.Framework;
using Terraria.UI;

namespace DaeLib.Geometry {
	public class Rect {
		public Vector2 Position = new Vector2(0, 0);
		public Vector2 Dimensions = new Vector2(0, 0);
		public float X {
			get => Position.X;
			set => Position.X = value;
		}
		public float Y {
			get => Position.Y;
			set => Position.Y = value;
		}
		public float Width {
			get => Dimensions.X;
			set => Dimensions.X = value;
		}
		public float Height {
			get => Dimensions.Y;
			set => Dimensions.Y = value;
		}
		public float Left => Position.X;
		public float Top => Position.Y;
		public float Right => Position.X + Dimensions.X;
		public float Bottom => Position.Y + Dimensions.Y;
		public Rect(float x, float y, float width, float height) {
			Position.X = x;
			Position.Y = y;
			Dimensions.X = width;
			Dimensions.Y = height;
		}
		public Rect(float x, float y, Vector2 dimensions) : this(x, y, dimensions.X, dimensions.Y) { }
		public Rect(Vector2 position, float width, float height) : this(position.X, position.Y, width, height) { }
		public Rect(Vector2 position, Vector2 dimensions) : this(position.X, position.Y, dimensions.X, dimensions.Y) { }
		public Rect(float width, float height) : this(0, 0, width, height) { }
		public Rect(Vector2 dimensions) : this(0, 0, dimensions.X, dimensions.Y) { }
		public Rect() : this(0, 0, 0, 0) { }
		public Rect(Rect rect) : this(rect.X, rect.Y, rect.Width, rect.Height) { }
		public Rect(CalculatedStyle style) : this(style.X, style.Y, style.Width, style.Height) { }
		public Rect Clone() => new Rect(Position.X, Position.Y, Dimensions.X, Dimensions.Y);

		public Vector2 ClonePosition() => new Vector2(Position.X, Position.Y);
		public Vector2 CloneDimensions() => new Vector2(Dimensions.X, Dimensions.Y);

		public void SetDimensions(Vector2 dimensions) {
			Width = dimensions.X;
			Height = dimensions.Y;
		}

		public void SetPosition(Vector2 position) {
			X = position.X;
			Y = position.Y;
		}

		public bool Contains(float x, float y) => x >= X && y >= Y && x <= Right && y <= Bottom;
		public bool Contains(Vector2 vector) => Contains(vector.X, vector.Y);
		public bool Contains(Point point) => Contains(point.X, point.Y);
		public Vector2 Center() => Position + Dimensions / 2;
	}
}

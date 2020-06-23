using DaeLib.Geometry;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DaeLib.Graphics {
	/// <summary>
	/// This class slices up a Texture2D into 4 corners, 4 stretchable sides, and a stretchable middle texture.
	/// </summary>
	public class ScalableTexture2D {
		public readonly Texture2D Texture;
		private readonly int[] widths = new int[3];
		private readonly int[] heights = new int[3];
		private readonly Rectangle[,] sourceRects = new Rectangle[3, 3];

		public ScalableTexture2D(Texture2D texture, int leftWidth, int rightWidth, int topHeight, int bottomHeight) {
			Texture = texture;

			int[] sourceX = { 0, leftWidth, texture.Width - rightWidth };
			int[] sourceY = { 0, topHeight, texture.Height - bottomHeight };
			int[] w = { leftWidth, texture.Width - leftWidth - rightWidth, rightWidth };
			int[] h = { topHeight, texture.Height - topHeight - bottomHeight, bottomHeight };

			for (int y = 0; y < 3; y++) {
				for (int x = 0; x < 3; x++) {
					sourceRects[y, x] = new Rectangle(sourceX[x], sourceY[y], w[x], h[y]);
				}
			}

			widths = w;
			heights = h;
		}

		public ScalableTexture2D(Texture2D texture, int cornerWidth, int cornerHeight) : this(texture, cornerWidth, cornerWidth, cornerHeight, cornerHeight) { }

		public ScalableTexture2D(Texture2D texture, int cornerSize) : this(texture, cornerSize, cornerSize) { }

		public void Draw(SpriteBatch spriteBatch, Rect rect, Color color, float scale = 1f) {
			float innerWidth = rect.Width - (widths[0] + widths[2]) * scale;
			float innerHeight = rect.Height - (heights[0] + heights[2]) * scale;

			float[] targetX = new float[3];
			targetX[0] = rect.X;
			targetX[1] = targetX[0] + widths[0] * scale;
			targetX[2] = targetX[1] + innerWidth;

			float[] targetY = new float[3];
			targetY[0] = rect.Y;
			targetY[1] = targetY[0] + heights[0] * scale;
			targetY[2] = targetY[1] + innerHeight;

			for (int y = 0; y < 3; y++) {
				for (int x = 0; x < 3; x++) {
					Rectangle sourceRect = sourceRects[y, x];

					float horzScale = x == 1 ? innerWidth / widths[1] : scale;
					float vertScale = y == 1 ? innerHeight / heights[1] : scale;

					spriteBatch.Draw(
						Texture,
						new Vector2(targetX[x], targetY[y]),
						sourceRect,
						color,
						0,
						Vector2.Zero,
						new Vector2(horzScale, vertScale),
						SpriteEffects.None,
						0
					);
				}
			}
		}
	}
}

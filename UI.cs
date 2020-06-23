using DaeLib.Geometry;
using DaeLib.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;

namespace DaeLib.UI {
	public class UIFocusGroup {
		private readonly List<UIInputElement> Targets = new List<UIInputElement>();
		public UIInputElement FocusTarget;
		public void Add(UIInputElement target) {
			if (Targets.Contains(target))
				throw new Exception($"{nameof(UIInputElement)} already in group.");

			Targets.Add(target);
			target.Group = this;
			if (FocusTarget == null)
				FocusTarget = target;
		}

		public bool Remove(UIInputElement target) {
			return Targets.Remove(target);
		}

		public void Focus(UIInputElement target) {
			if (!Targets.Contains(target))
				throw new Exception($"{nameof(UIInputElement)} not in group.");

			FocusTarget = target;
		}

		public void FocusNext(bool reverse = false) {
			if (Targets.Count == 0)
				return;

			if (Targets.Count == 1) {
				FocusTarget = Targets[0];
				return;
			}

			int targetIndex = (Targets.IndexOf(FocusTarget) + (reverse ? -1 : 1) + Targets.Count) % Targets.Count;
			FocusTarget = Targets[targetIndex];
			Main.PlaySound(SoundID.MenuTick);
		}

		public void CheckTab() {
			// If game is not focused, or tab is not held, or tab was held on the last frame: return
			if (!Main.hasFocus || !Main.keyState.IsKeyDown(Keys.Tab) || Main.oldKeyState.IsKeyDown(Keys.Tab))
				return;

			Main.clrInput();
			FocusNext(Main.keyState.IsKeyDown(Keys.LeftShift) || Main.keyState.IsKeyDown(Keys.RightShift));
		}
	}

	public class UIInputElement : UIElement {
		public UIFocusGroup Group = null;
		public bool Focused => Main.hasFocus && Group?.FocusTarget == this;
		public bool Readonly = false;

		public override void Click(UIMouseEvent evt) {
			Main.clrInput();
			Group?.Focus(this);
		}
	}

	public class UICheckbox : UIInputElement {
		public static readonly float DEFAULT_HEIGHT = 24;
		private static readonly string CHECKMARK = "✓";
		private static ScalableTexture2D CheckboxTexture;
		public bool Checked = false;

		public Color ActiveCheckboxColor = Color.White;
		public Color InactiveCheckboxColor = Color.White * 0.3f;
		public Color CheckmarkColor = Color.White;
		public Color ReadonlyCheckmarkColor = Color.White * 0.3f;
		public UICheckbox(bool value = false, bool isReadonly = false) : base() {
			Checked = value;
			Readonly = isReadonly;
			Width.Set(DEFAULT_HEIGHT, 0);
			Height.Set(DEFAULT_HEIGHT, 0);
		}
		public override void Click(UIMouseEvent evt) {
			base.Click(evt);
			if (!Readonly) {
				Checked = !Checked;
				Main.PlaySound(SoundID.MenuTick);
			}
		}
		public override void OnInitialize() {
			CheckboxTexture = new ScalableTexture2D(ModContent.GetTexture("DaeLib/PanelOutset"), 6);
		}
		public override void Draw(SpriteBatch spriteBatch) {
			if (!Readonly && Focused && Main.keyState.IsKeyDown(Keys.Space) && !Main.oldKeyState.IsKeyDown(Keys.Space))
				Checked = !Checked;

			bool isActive = !Readonly && (Focused || IsMouseHovering);
			
			Rect checkboxRect = new Rect(GetInnerDimensions());
			CheckboxTexture.Draw(spriteBatch, checkboxRect, isActive ? ActiveCheckboxColor : InactiveCheckboxColor);

			if (Checked) {
				Vector2 checkmarkPosition = checkboxRect.Center();
				checkmarkPosition.X += 2;
				checkmarkPosition.Y += 4;

				Utils.DrawBorderString(spriteBatch, CHECKMARK, checkmarkPosition, Readonly ? ReadonlyCheckmarkColor : CheckmarkColor, 1, 0.5f, 0.5f);
			}
		}
	}
	public class UILabeledCheckbox : UIElement {
		public static readonly float DEFAULT_HEIGHT = UICheckbox.DEFAULT_HEIGHT;
		public UICheckbox Checkbox;
		public UIText UILabel;
		public float Gap = 2;

		public LocalizedText Label;
		public UILabeledCheckbox(LocalizedText label, bool isChecked = false, bool isReadonly = false) : base() {
			Label = label;
			Checkbox = new UICheckbox(isChecked, isReadonly);
		}

		public override void OnInitialize() {
			MinWidth = StyleDimension.Fill;
			MinHeight.Set(DEFAULT_HEIGHT, 0);

			Append(Checkbox);

			UILabel = new UIText(Label);
			UILabel.Left.Set(DEFAULT_HEIGHT + Gap, 0f);
			UILabel.VAlign = 0.5f;
			Append(UILabel);
		}

		public override void Click(UIMouseEvent evt) {
			Checkbox.Click(evt);
		}
	}

	public class UITextInput : UIInputElement {
		public static readonly float DEFAULT_HEIGHT = 36;
		private static ScalableTexture2D InputTexture;
		private static readonly float LineHeight = 20;

		public LocalizedText Placeholder;
		public string Value = "";
		public bool Sensitive = false;
		public Func<string, string> Filter = null;

		private int textBlinkerCount = 0;
		private bool textBlinkerState = false;

		public UITextInput(LocalizedText placeholder = null, string value = "") {
			Placeholder = placeholder;
			Value = value;
		}

		public override void OnInitialize() {
			InputTexture = new ScalableTexture2D(ModContent.GetTexture("DaeLib/PanelOutset"), 6);
			MinWidth = StyleDimension.Fill;
			Height.Set(DEFAULT_HEIGHT, 0);
		}

		public override void Draw(SpriteBatch spriteBatch) {
			if (Focused) {
				PlayerInput.WritingText = true;
				Main.instance.HandleIME();
				string updatedString = Main.GetInputText(Value);
				if (Filter != null)
					updatedString = Filter(updatedString);

				if (updatedString != Value) {
					Value = updatedString;
					Main.PlaySound(SoundID.MenuTick);
				}

				if (++textBlinkerCount >= 20) {
					textBlinkerState = !textBlinkerState;
					textBlinkerCount = 0;
				}
			} else {
				textBlinkerState = false;
			}

			Rect dimensions = new Rect(GetInnerDimensions());
			InputTexture.Draw(spriteBatch, dimensions, Focused ? Color.White : Color.White * 0.7f);

			Vector2 StringPosition = dimensions.Position;
			StringPosition.X += 8;
			StringPosition.Y += (dimensions.Height - LineHeight) / 2;

			if (Value.Length == 0 && !Focused && Placeholder != null) {
				Utils.DrawBorderString(spriteBatch, Placeholder.Value, StringPosition, Color.Gray);
			} else {
				string displayString = (Sensitive ? Regex.Replace(Value, ".", "*") : Value) + (textBlinkerState ? "|" : "");
				Utils.DrawBorderString(spriteBatch, displayString, StringPosition, Color.White);
			}
		}
	}

	public class UILabeledTextInput : UIElement {
		public static readonly float LABEL_HEIGHT = 24;
		public static readonly float DEFAULT_HEIGHT = UITextInput.DEFAULT_HEIGHT + LABEL_HEIGHT;
		public UITextInput Input;

		private readonly LocalizedText Label;

		public UILabeledTextInput(LocalizedText label, LocalizedText placeholder = null, string value = "") {
			Label = label;
			Input = new UITextInput(placeholder, value);
		}

		public override void OnInitialize() {
			Height.Set(DEFAULT_HEIGHT, 0);

			UIText UILabel = new UIText(Label);
			UILabel.Top.Set(2, 0);
			UILabel.Left.Set(-2, 0);
			UILabel.VAlign = 0;
			UILabel.Height.Set(LABEL_HEIGHT, 0);
			Append(UILabel);

			Input.Top.Set(LABEL_HEIGHT, 0);
			Append(Input);
		}

		public override void Click(UIMouseEvent evt) {
			Input.Click(evt);
		}
	}

	public class UILargeButton : UITextPanel<LocalizedText> {
		public UILargeButton(LocalizedText text, MouseEvent onClick) : base(text, 0.7f, true) {
			Width.Set(0, 1);
			Height.Set(50, 0);
			OnMouseOver += UILargeButton_OnMouseOver;
			OnMouseOut += UILargeButton_OnMouseOut;
			OnClick += onClick;
		}

		private void UILargeButton_OnMouseOver(UIMouseEvent evt, UIElement listeningElement) {
			Main.PlaySound(SoundID.MenuTick);
			BackgroundColor = new Color(73, 94, 171);
		}

		private void UILargeButton_OnMouseOut(UIMouseEvent evt, UIElement listeningElement) {
			BackgroundColor = new Color(63, 82, 151) * 0.7f;
		}
	}
}

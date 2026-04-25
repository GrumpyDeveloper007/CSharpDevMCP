using ApplicationController.FlaUI;
using FlaUI.Core.Input;
using ModelContextProtocol.Server;
using System.ComponentModel;

namespace ApplicationController.MCP
{
    /// <summary>
    /// Provides MCP tools for interacting with the application under test, such as getting snapshots, clicking elements, and setting text.
    /// </summary>
    [McpServerToolType]
    public class ApplicationControllerMCP(SessionManager _sessionManager, SnapshotBuilder _snapshotBuilder, ElementRegistry _elementRegistry)
    {

        [McpServerTool, Description("Get a Snapshot of the application")]
        public string GetSnapshot()
        {
            try
            {
                var windows = _sessionManager.ListWindows();
                var (handle, title, processName) = windows.SingleOrDefault(o => o.title.StartsWith(StaticSettings.SettingValues.ApplicationName, StringComparison.InvariantCultureIgnoreCase));
                var window = _sessionManager.GetWindow(handle);
                if (window == null) return $"Could not find window.";
                var snapshotText = _snapshotBuilder.BuildSnapshot(handle, window);
                return snapshotText;
            }
            catch (Exception ex)
            {
                return $"Exception: {ex.Message}";
            }
        }

        [McpServerTool, Description("Get the text content of an element. Returns the element's Name property, or for text inputs, the current value.")]
        public string GetText(string elementRefId)
        {
            try
            {
                var element = _elementRegistry.GetElement(elementRefId);
                if (element == null) return $"Element with ref {elementRefId} not found. Run GetSnapshot to refresh element refs.";

                string? text = null;
                // Try Value pattern first (for text inputs)
                if (element.Patterns.Value.IsSupported)
                {
                    text = element.Patterns.Value.Pattern.Value.ValueOrDefault;
                }

                // Fall back to Name property
                if (string.IsNullOrEmpty(text))
                {
                    text = element.Properties.Name.ValueOrDefault;
                }

                // Try Text pattern
                if (string.IsNullOrEmpty(text) && element.Patterns.Text.IsSupported)
                {
                    text = element.Patterns.Text.Pattern.DocumentRange.GetText(-1);
                }

                return text ?? "";
            }
            catch (Exception ex)
            {
                return $"Exception: {ex.Message}";
            }
        }

        [McpServerTool, Description("Set the text content of an element. Returns the element's Name property, or for text inputs, the current value.")]
        public string SetText(string elementRefId, string text)
        {
            try
            {
                var element = _elementRegistry.GetElement(elementRefId);
                if (element == null) return $"Element with ref {elementRefId} not found. Run GetSnapshot to refresh element refs.";
                element.Focus();
                Thread.Sleep(50); // Small delay to ensure focus

                // Type the text
                Keyboard.Type(text);

                var target = string.IsNullOrEmpty(elementRefId) ? "focused element" : elementRefId;
                var action = "Typed";
                return $"{action} \"{text}\" into {target}";

            }
            catch (Exception ex)
            {
                return $"Exception: {ex.Message}";
            }
        }
        /*
        //Element ref to capture. If omitted, captures the whole window.
        [McpServerTool, Description("Take a screenshot of a window or specific element. Returns the image as base64-encoded PNG.")]
        public string GetScreenshot()
        {
            try
            {
                var windows = _sessionManager.ListWindows();
                var app = windows.SingleOrDefault(o => o.title == StaticSettings.SettingValues.ApplicationName);
                var window = _sessionManager.GetWindow(app.handle);
                if (window == null) return $"Could not find window.";

                CaptureImage capture;
                capture = Capture.Element(window);

                // TODO:
                //if (!string.IsNullOrEmpty(elementRefId))
                //{
                //    var element = _elementRegistry.GetElement(refId);
                //    if (element == null)
                //    {
                //        return Task.FromResult(ErrorResult($"Element not found: {refId}"));
                //    }
                //    capture = Capture.Element(element);
                //}

                using var stream = new MemoryStream();
                capture.Bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                var imageData = stream.ToArray();

                //TODO: return Task.FromResult(ImageResult(imageData, "image/png"));
                return "TODO:";

            }
            catch (Exception ex)
            {
                return $"Exception: {ex.Message}";
            }
        }
        */

        [McpServerTool, Description("click on a element")]
        public string Click(string elementRefId)
        {
            try
            {
                var doubleClick = false;
                var leftClick = true;
                return Click(leftClick, doubleClick, elementRefId);
            }
            catch (Exception ex)
            {
                return $"Exception: {ex.Message}";
            }
        }

        [McpServerTool, Description("Right click on a element")]
        public string RightClick(string elementRefId)
        {
            try
            {
                var doubleClick = false;
                var leftClick = false;
                return Click(leftClick, doubleClick, elementRefId);
            }
            catch (Exception ex)
            {
                return $"Exception: {ex.Message}";
            }
        }

        [McpServerTool, Description("Double click on a element")]
        public string DoubleClick(string elementRefId)
        {
            try
            {
                var doubleClick = true;
                var leftClick = true;
                return Click(leftClick, doubleClick, elementRefId);
            }
            catch (Exception ex)
            {
                return $"Exception: {ex.Message}";
            }
        }

        private string Click(bool leftClick, bool doubleClick, string elementRefId)
        {
            var element = _elementRegistry.GetElement(elementRefId);
            if (element == null) return $"Element with ref {elementRefId} not found. Run GetSnapshot to refresh element refs.";
            var elementName = element.Properties.Name.ValueOrDefault ?? elementRefId;

            // Try Invoke pattern first (most reliable for buttons)
            if (leftClick && !doubleClick && element.Patterns.Invoke.IsSupported)
            {
                element.Patterns.Invoke.Pattern.Invoke();
                return $"Invoked {elementName}";
            }

            // Try Toggle pattern for checkboxes
            if (leftClick && !doubleClick && element.Patterns.Toggle.IsSupported)
            {
                element.Patterns.Toggle.Pattern.Toggle();
                var newState = element.Patterns.Toggle.Pattern.ToggleState.ValueOrDefault;
                return $"Toggled {elementName} to {newState}";
            }

            // Try SelectionItem pattern for list items
            if (leftClick && !doubleClick && element.Patterns.SelectionItem.IsSupported)
            {
                element.Patterns.SelectionItem.Pattern.Select();
                return $"Selected {elementName}";
            }

            // Fall back to mouse click
            var clickPoint = element.GetClickablePoint();

            var mouseButton = leftClick switch
            {
                false => MouseButton.Right,
                //"middle" => MouseButton.Middle,
                _ => MouseButton.Left
            };

            if (doubleClick)
            {
                Mouse.DoubleClick(clickPoint, mouseButton);
                return $"Double-clicked {elementName}";
            }
            else
            {
                Mouse.Click(clickPoint, mouseButton);
                return $"Clicked {elementName}";
            }
        }
    }
}
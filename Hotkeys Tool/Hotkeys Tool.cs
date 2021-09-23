using cAlgo.API;
using System;
using System.Linq;

namespace cAlgo.Robots
{
    [Robot(TimeZone = TimeZones.UTC, AccessRights = AccessRights.None)]
    public class HotkeysTool : Robot
    {
        private double? _stopLossInPips = null;
        private double? _takeProfitInPips = null;

        private Color _drawingColor;

        private ScrollViewer _hotkeysTableScrollViewer;

        private const string _drawingObjectsNamePrefix = "HotkeysTool";

        [Parameter("Volume (Units)", DefaultValue = 1000, Step = 1, MinValue = 0, Group = "Trading")]
        public double Volume { get; set; }

        [Parameter("Stop Loss (Pips)", DefaultValue = 30, Step = 1, MinValue = 0, Group = "Trading")]
        public double StopLossInPips { get; set; }

        [Parameter("Take Profit (Pips)", DefaultValue = 0, Step = 1, MinValue = 0, Group = "Trading")]
        public double TakeProfitInPips { get; set; }

        [Parameter("Pending Order Distance (Pips)", DefaultValue = 50, Step = 1, MinValue = 0, Group = "Trading")]
        public double PendingOrderDistanceInPips { get; set; }

        [Parameter("Stop Limit Range (Pips)", DefaultValue = 50, Step = 1, MinValue = 0, Group = "Trading")]
        public double StopLimitRangeInPips { get; set; }

        [Parameter("Label", DefaultValue = "HotkeysTool", Group = "Trading")]
        public string Label { get; set; }

        [Parameter("Modifier Key", DefaultValue = ModifierKeys.Shift, Group = "Trading")]
        public ModifierKeys TradingModifierKey { get; set; }

        [Parameter("Buy Market Key", DefaultValue = Key.B, Group = "Trading")]
        public Key BuyMarketKey { get; set; }

        [Parameter("Sell Market Key", DefaultValue = Key.S, Group = "Trading")]
        public Key SellMarketKey { get; set; }

        [Parameter("Close All Market Key", DefaultValue = Key.C, Group = "Trading")]
        public Key CloseAllMarketKey { get; set; }

        [Parameter("Buy Limit Key", DefaultValue = Key.Q, Group = "Trading")]
        public Key BuyLimitKey { get; set; }

        [Parameter("Sell Limit Key", DefaultValue = Key.W, Group = "Trading")]
        public Key SellLimitKey { get; set; }

        [Parameter("Cancel All Limit Key", DefaultValue = Key.Z, Group = "Trading")]
        public Key CancelAllLimitKey { get; set; }

        [Parameter("Buy Stop Key", DefaultValue = Key.E, Group = "Trading")]
        public Key BuyStopKey { get; set; }

        [Parameter("Sell Stop Key", DefaultValue = Key.R, Group = "Trading")]
        public Key SellStopKey { get; set; }

        [Parameter("Cancel All Stop Key", DefaultValue = Key.X, Group = "Trading")]
        public Key CancelAllStopKey { get; set; }

        [Parameter("Buy Stop Limit Key", DefaultValue = Key.T, Group = "Trading")]
        public Key BuyStopLimitKey { get; set; }

        [Parameter("Sell Stop Limit Key", DefaultValue = Key.Y, Group = "Trading")]
        public Key SellStopLimitKey { get; set; }

        [Parameter("Cancel All Stop Limit Key", DefaultValue = Key.V, Group = "Trading")]
        public Key CancelAllStopLimitKey { get; set; }

        [Parameter("Color", DefaultValue = "Red", Group = "Drawing")]
        public string DrawingColor { get; set; }

        [Parameter("Color Alpha", DefaultValue = 130, MinValue = 0, MaxValue = 255, Group = "Drawing")]
        public int DrawingColorAlpha { get; set; }

        [Parameter("Modifier Key", DefaultValue = ModifierKeys.Alt, Group = "Drawing")]
        public ModifierKeys DrawingModifierKey { get; set; }

        [Parameter("Vertical Line Key", DefaultValue = Key.D, Group = "Drawing")]
        public Key VerticalLineKey { get; set; }

        [Parameter("Horizontal Line Key", DefaultValue = Key.F, Group = "Drawing")]
        public Key HorizontalLineKey { get; set; }

        [Parameter("Trend Line Key", DefaultValue = Key.G, Group = "Drawing")]
        public Key TrendLineKey { get; set; }

        [Parameter("Rectangle Key", DefaultValue = Key.H, Group = "Drawing")]
        public Key RectangleKey { get; set; }

        [Parameter("Triangle Key", DefaultValue = Key.J, Group = "Drawing")]
        public Key TriangleKey { get; set; }

        [Parameter("Ellipse Key", DefaultValue = Key.K, Group = "Drawing")]
        public Key EllipseKey { get; set; }

        [Parameter("Equidistant Channel Key", DefaultValue = Key.L, Group = "Drawing")]
        public Key EquidistantChannelKey { get; set; }

        [Parameter("Andrews Pitchfork Key", DefaultValue = Key.A, Group = "Drawing")]
        public Key AndrewsPitchforkKey { get; set; }

        [Parameter("Fibonacci Retracement Key", DefaultValue = Key.R, Group = "Drawing")]
        public Key FibonacciRetracementKey { get; set; }

        [Parameter("Fibonacci Fan Key", DefaultValue = Key.T, Group = "Drawing")]
        public Key FibonacciFanKey { get; set; }

        [Parameter("Fibonacci Expansion Key", DefaultValue = Key.U, Group = "Drawing")]
        public Key FibonacciExpansionKey { get; set; }

        [Parameter("Modifier Key", DefaultValue = ModifierKeys.Shift, Group = "Chart Display")]
        public ModifierKeys ChartDisplayModifierKey { get; set; }

        [Parameter("Deal Map", DefaultValue = Key.D, Group = "Chart Display")]
        public Key DealMapKey { get; set; }

        [Parameter("Show/Hide Key", DefaultValue = Key.M, Group = "Hotkeys Table")]
        public Key HotkeysTableShowHideKey { get; set; }

        [Parameter("Show/Hide Modifier Key", DefaultValue = ModifierKeys.None, Group = "Hotkeys Table")]
        public ModifierKeys HotkeysTableShowHideModifierKey { get; set; }

        [Parameter("Horizontal Alignment", DefaultValue = HorizontalAlignment.Right, Group = "Hotkeys Table")]
        public HorizontalAlignment HorizontalAlignment { get; set; }

        [Parameter("Vertical Alignment", DefaultValue = VerticalAlignment.Top, Group = "Hotkeys Table")]
        public VerticalAlignment VerticalAlignment { get; set; }

        [Parameter("Background Color", DefaultValue = "Yellow", Group = "Hotkeys Table")]
        public string BackgroundColor { get; set; }

        [Parameter("Text Color", DefaultValue = "Black", Group = "Hotkeys Table")]
        public string TextColor { get; set; }

        [Parameter("Opacity", DefaultValue = 0.5, MinValue = 0, MaxValue = 1, Group = "Hotkeys Table")]
        public double Opacity { get; set; }

        [Parameter("Margin", DefaultValue = 5, MinValue = 0, Group = "Hotkeys Table")]
        public double Margin { get; set; }

        [Parameter("Font Size", DefaultValue = 14, MinValue = 0, Group = "Hotkeys Table")]
        public double FontSize { get; set; }

        [Parameter("Font Weight", DefaultValue = FontWeight.Normal, Group = "Hotkeys Table")]
        public FontWeight FontWeight { get; set; }

        protected override void OnStart()
        {
            if (StopLossInPips > 0) _stopLossInPips = StopLossInPips;
            if (TakeProfitInPips > 0) _takeProfitInPips = TakeProfitInPips;

            PendingOrderDistanceInPips *= Symbol.PipSize;

            _drawingColor = GetColor(DrawingColor, DrawingColorAlpha);

            AddTradingHotkeys();
            AddDrawingHotkeys();
            AddChartDisplayHotkeys();

            ShowHotkeysOnChart();

            Chart.AddHotkey(ShowHideHotkeysTable, HotkeysTableShowHideKey, HotkeysTableShowHideModifierKey);
        }

        private void ShowHideHotkeysTable(ChartKeyboardEventArgs obj)
        {
            _hotkeysTableScrollViewer.IsVisible = !_hotkeysTableScrollViewer.IsVisible;
        }

        private void ShowHotkeysOnChart()
        {
            var grid = new Grid(29, 2)
            {
                HorizontalAlignment = HorizontalAlignment,
                VerticalAlignment = VerticalAlignment,
                BackgroundColor = GetColor(BackgroundColor)
            };

            var textBlocksStyle = new Style();

            textBlocksStyle.Set(ControlProperty.Margin, Margin);
            textBlocksStyle.Set(ControlProperty.FontSize, FontSize);
            textBlocksStyle.Set(ControlProperty.FontWeight, FontWeight);
            textBlocksStyle.Set(ControlProperty.ForegroundColor, GetColor(TextColor));

            grid.AddChild(new TextBlock { Text = "Trading", HorizontalAlignment = HorizontalAlignment.Center, Style = textBlocksStyle }, 0, 0, 1, 2);

            grid.AddChild(new TextBlock { Text = "Buy Market", Style = textBlocksStyle }, 1, 0);
            grid.AddChild(new TextBlock { Text = GetHotkeyText(BuyMarketKey, TradingModifierKey), Style = textBlocksStyle }, 1, 1);

            grid.AddChild(new TextBlock { Text = "Sell Market", Style = textBlocksStyle }, 2, 0);
            grid.AddChild(new TextBlock { Text = GetHotkeyText(SellMarketKey, TradingModifierKey), Style = textBlocksStyle }, 2, 1);

            grid.AddChild(new TextBlock { Text = "Close All Market", Style = textBlocksStyle }, 3, 0);
            grid.AddChild(new TextBlock { Text = GetHotkeyText(CloseAllMarketKey, TradingModifierKey), Style = textBlocksStyle }, 3, 1);

            grid.AddChild(new TextBlock { Text = "Buy Limit", Style = textBlocksStyle }, 4, 0);
            grid.AddChild(new TextBlock { Text = GetHotkeyText(BuyLimitKey, TradingModifierKey), Style = textBlocksStyle }, 4, 1);

            grid.AddChild(new TextBlock { Text = "Sell Limit", Style = textBlocksStyle }, 5, 0);
            grid.AddChild(new TextBlock { Text = GetHotkeyText(SellLimitKey, TradingModifierKey), Style = textBlocksStyle }, 5, 1);

            grid.AddChild(new TextBlock { Text = "Cancel All Limit", Style = textBlocksStyle }, 6, 0);
            grid.AddChild(new TextBlock { Text = GetHotkeyText(CancelAllLimitKey, TradingModifierKey), Style = textBlocksStyle }, 6, 1);

            grid.AddChild(new TextBlock { Text = "Buy Stop", Style = textBlocksStyle }, 7, 0);
            grid.AddChild(new TextBlock { Text = GetHotkeyText(BuyStopKey, TradingModifierKey), Style = textBlocksStyle }, 7, 1);

            grid.AddChild(new TextBlock { Text = "Sell Stop", Style = textBlocksStyle }, 8, 0);
            grid.AddChild(new TextBlock { Text = GetHotkeyText(SellStopKey, TradingModifierKey), Style = textBlocksStyle }, 8, 1);

            grid.AddChild(new TextBlock { Text = "Cancel All Stop", Style = textBlocksStyle }, 9, 0);
            grid.AddChild(new TextBlock { Text = GetHotkeyText(CancelAllStopKey, TradingModifierKey), Style = textBlocksStyle }, 9, 1);

            grid.AddChild(new TextBlock { Text = "Buy Stop Limit", Style = textBlocksStyle }, 10, 0);
            grid.AddChild(new TextBlock { Text = GetHotkeyText(BuyStopLimitKey, TradingModifierKey), Style = textBlocksStyle }, 10, 1);

            grid.AddChild(new TextBlock { Text = "Sell Stop Limit", Style = textBlocksStyle }, 11, 0);
            grid.AddChild(new TextBlock { Text = GetHotkeyText(SellStopLimitKey, TradingModifierKey), Style = textBlocksStyle }, 11, 1);

            grid.AddChild(new TextBlock { Text = "Cancel All Stop Limit", Style = textBlocksStyle }, 12, 0);
            grid.AddChild(new TextBlock { Text = GetHotkeyText(CancelAllStopLimitKey, TradingModifierKey), Style = textBlocksStyle }, 12, 1);

            grid.AddChild(new TextBlock { Text = "Drawing", HorizontalAlignment = HorizontalAlignment.Center, Style = textBlocksStyle }, 13, 0, 1, 2);

            grid.AddChild(new TextBlock { Text = "Vertical Line", Style = textBlocksStyle }, 14, 0);
            grid.AddChild(new TextBlock { Text = GetHotkeyText(VerticalLineKey, DrawingModifierKey), Style = textBlocksStyle }, 14, 1);

            grid.AddChild(new TextBlock { Text = "Horizontal Line", Style = textBlocksStyle }, 15, 0);
            grid.AddChild(new TextBlock { Text = GetHotkeyText(HorizontalLineKey, DrawingModifierKey), Style = textBlocksStyle }, 15, 1);

            grid.AddChild(new TextBlock { Text = "Trend Line", Style = textBlocksStyle }, 16, 0);
            grid.AddChild(new TextBlock { Text = GetHotkeyText(TrendLineKey, DrawingModifierKey), Style = textBlocksStyle }, 16, 1);

            grid.AddChild(new TextBlock { Text = "Rectangle", Style = textBlocksStyle }, 17, 0);
            grid.AddChild(new TextBlock { Text = GetHotkeyText(RectangleKey, DrawingModifierKey), Style = textBlocksStyle }, 17, 1);

            grid.AddChild(new TextBlock { Text = "Triangle", Style = textBlocksStyle }, 18, 0);
            grid.AddChild(new TextBlock { Text = GetHotkeyText(TriangleKey, DrawingModifierKey), Style = textBlocksStyle }, 18, 1);

            grid.AddChild(new TextBlock { Text = "Ellipse", Style = textBlocksStyle }, 19, 0);
            grid.AddChild(new TextBlock { Text = GetHotkeyText(EllipseKey, DrawingModifierKey), Style = textBlocksStyle }, 19, 1);

            grid.AddChild(new TextBlock { Text = "Equidistant Channel", Style = textBlocksStyle }, 20, 0);
            grid.AddChild(new TextBlock { Text = GetHotkeyText(EquidistantChannelKey, DrawingModifierKey), Style = textBlocksStyle }, 20, 1);

            grid.AddChild(new TextBlock { Text = "Andrews Pitchfork", Style = textBlocksStyle }, 21, 0);
            grid.AddChild(new TextBlock { Text = GetHotkeyText(AndrewsPitchforkKey, DrawingModifierKey), Style = textBlocksStyle }, 21, 1);

            grid.AddChild(new TextBlock { Text = "Fibonacci Retracement", Style = textBlocksStyle }, 22, 0);
            grid.AddChild(new TextBlock { Text = GetHotkeyText(FibonacciRetracementKey, DrawingModifierKey), Style = textBlocksStyle }, 22, 1);

            grid.AddChild(new TextBlock { Text = "Fibonacci Fan", Style = textBlocksStyle }, 23, 0);
            grid.AddChild(new TextBlock { Text = GetHotkeyText(FibonacciFanKey, DrawingModifierKey), Style = textBlocksStyle }, 23, 1);

            grid.AddChild(new TextBlock { Text = "Fibonacci Expansion", Style = textBlocksStyle }, 24, 0);
            grid.AddChild(new TextBlock { Text = GetHotkeyText(FibonacciExpansionKey, DrawingModifierKey), Style = textBlocksStyle }, 24, 1);

            grid.AddChild(new TextBlock { Text = "Chart Display", HorizontalAlignment = HorizontalAlignment.Center, Style = textBlocksStyle }, 25, 0, 1, 2);

            grid.AddChild(new TextBlock { Text = "Show/Hide Deal Map", Style = textBlocksStyle }, 26, 0);
            grid.AddChild(new TextBlock { Text = GetHotkeyText(DealMapKey, ChartDisplayModifierKey), Style = textBlocksStyle }, 26, 1);

            grid.AddChild(new TextBlock { Text = "Others", HorizontalAlignment = HorizontalAlignment.Center, Style = textBlocksStyle }, 27, 0, 1, 2);

            grid.AddChild(new TextBlock { Text = "Show/Hide Table", Style = textBlocksStyle }, 28, 0);
            grid.AddChild(new TextBlock { Text = GetHotkeyText(HotkeysTableShowHideKey, HotkeysTableShowHideModifierKey), Style = textBlocksStyle }, 28, 1);

            _hotkeysTableScrollViewer = new ScrollViewer()
            {
                Content = grid,
                HorizontalAlignment = HorizontalAlignment,
                VerticalAlignment = VerticalAlignment,
                Opacity = Opacity
            };

            Chart.AddControl(_hotkeysTableScrollViewer);
        }

        private void AddTradingHotkeys()
        {
            Chart.AddHotkey(() => MarketOrderKeyHandler(TradeType.Buy), BuyMarketKey, TradingModifierKey);
            Chart.AddHotkey(() => MarketOrderKeyHandler(TradeType.Sell), SellMarketKey, TradingModifierKey);
            Chart.AddHotkey(CloseMarketOrders, CloseAllMarketKey, TradingModifierKey);
            Chart.AddHotkey(() => PlaceOrder(PendingOrderType.Limit, TradeType.Buy), BuyLimitKey, TradingModifierKey);
            Chart.AddHotkey(() => PlaceOrder(PendingOrderType.Limit, TradeType.Sell), SellLimitKey, TradingModifierKey);
            Chart.AddHotkey(() => CancelOrders(PendingOrderType.Limit), CancelAllLimitKey, TradingModifierKey);
            Chart.AddHotkey(() => PlaceOrder(PendingOrderType.Stop, TradeType.Buy), BuyStopKey, TradingModifierKey);
            Chart.AddHotkey(() => PlaceOrder(PendingOrderType.Stop, TradeType.Sell), SellStopKey, TradingModifierKey);
            Chart.AddHotkey(() => CancelOrders(PendingOrderType.Stop), CancelAllStopKey, TradingModifierKey);
            Chart.AddHotkey(() => PlaceOrder(PendingOrderType.StopLimit, TradeType.Buy), BuyStopLimitKey, TradingModifierKey);
            Chart.AddHotkey(() => PlaceOrder(PendingOrderType.StopLimit, TradeType.Sell), SellStopLimitKey, TradingModifierKey);
            Chart.AddHotkey(() => CancelOrders(PendingOrderType.StopLimit), CancelAllStopLimitKey, TradingModifierKey);
        }

        private void AddDrawingHotkeys()
        {
            Chart.AddHotkey(() => Draw(ChartObjectType.VerticalLine), VerticalLineKey, DrawingModifierKey);
            Chart.AddHotkey(() => Draw(ChartObjectType.HorizontalLine), HorizontalLineKey, DrawingModifierKey);
            Chart.AddHotkey(() => Draw(ChartObjectType.TrendLine), TrendLineKey, DrawingModifierKey);
            Chart.AddHotkey(() => Draw(ChartObjectType.Rectangle), RectangleKey, DrawingModifierKey);
            Chart.AddHotkey(() => Draw(ChartObjectType.Triangle), TriangleKey, DrawingModifierKey);
            Chart.AddHotkey(() => Draw(ChartObjectType.Ellipse), EllipseKey, DrawingModifierKey);
            Chart.AddHotkey(() => Draw(ChartObjectType.EquidistantChannel), EquidistantChannelKey, DrawingModifierKey);
            Chart.AddHotkey(() => Draw(ChartObjectType.AndrewsPitchfork), AndrewsPitchforkKey, DrawingModifierKey);
            Chart.AddHotkey(() => Draw(ChartObjectType.FibonacciRetracement), FibonacciRetracementKey, DrawingModifierKey);
            Chart.AddHotkey(() => Draw(ChartObjectType.FibonacciFan), FibonacciFanKey, DrawingModifierKey);
            Chart.AddHotkey(() => Draw(ChartObjectType.FibonacciExpansion), FibonacciExpansionKey, DrawingModifierKey);
        }

        private void AddChartDisplayHotkeys()
        {
            Chart.AddHotkey(() => Chart.DisplaySettings.DealMap = !Chart.DisplaySettings.DealMap, DealMapKey, ChartDisplayModifierKey);
        }

        private void Draw(ChartObjectType type)
        {
            ChartObject chartObject = null;

            var barsToCover = new Lazy<int>(() => (Chart.LastVisibleBarIndex - Chart.FirstVisibleBarIndex) / 3);
            var priceToCover = new Lazy<double>(() => (Chart.TopY - Chart.BottomY) / 3);

            switch (type)
            {
                case ChartObjectType.VerticalLine:
                    chartObject = Chart.DrawVerticalLine(GetObjectName(type), Chart.FirstVisibleBarIndex + (Chart.LastVisibleBarIndex - Chart.FirstVisibleBarIndex) / 2, _drawingColor);
                    break;

                case ChartObjectType.HorizontalLine:
                    chartObject = Chart.DrawHorizontalLine(GetObjectName(type), Chart.BottomY + (Chart.TopY - Chart.BottomY) / 2, _drawingColor);
                    break;

                case ChartObjectType.TrendLine:
                    chartObject = Chart.DrawTrendLine(GetObjectName(type), Chart.FirstVisibleBarIndex + barsToCover.Value, Chart.BottomY + priceToCover.Value, Chart.LastVisibleBarIndex - barsToCover.Value, Chart.TopY - priceToCover.Value, _drawingColor);
                    break;

                case ChartObjectType.Rectangle:
                    chartObject = Chart.DrawRectangle(GetObjectName(type), Chart.FirstVisibleBarIndex + barsToCover.Value, Chart.BottomY + priceToCover.Value, Chart.LastVisibleBarIndex - barsToCover.Value, Chart.TopY - priceToCover.Value, _drawingColor);
                    break;

                case ChartObjectType.Triangle:
                    chartObject = Chart.DrawTriangle(GetObjectName(type), Chart.FirstVisibleBarIndex + barsToCover.Value, Chart.BottomY + priceToCover.Value, Chart.FirstVisibleBarIndex + (Chart.LastVisibleBarIndex - Chart.FirstVisibleBarIndex) / 2, Chart.TopY - priceToCover.Value,
                        Chart.LastVisibleBarIndex - barsToCover.Value, Chart.BottomY + priceToCover.Value, _drawingColor);
                    break;

                case ChartObjectType.Ellipse:
                    chartObject = Chart.DrawEllipse(GetObjectName(type), Chart.FirstVisibleBarIndex + barsToCover.Value, Chart.BottomY + priceToCover.Value, Chart.LastVisibleBarIndex - barsToCover.Value, Chart.TopY - priceToCover.Value, _drawingColor);
                    break;

                case ChartObjectType.EquidistantChannel:
                    chartObject = Chart.DrawEquidistantChannel(GetObjectName(type), Chart.FirstVisibleBarIndex + barsToCover.Value, Chart.BottomY + priceToCover.Value, Chart.LastVisibleBarIndex - barsToCover.Value, Chart.TopY - priceToCover.Value, priceToCover.Value / 2, _drawingColor);
                    break;

                case ChartObjectType.AndrewsPitchfork:
                    chartObject = Chart.DrawAndrewsPitchfork(GetObjectName(type), Chart.FirstVisibleBarIndex + barsToCover.Value, Chart.BottomY + priceToCover.Value, Chart.FirstVisibleBarIndex + (Chart.LastVisibleBarIndex - Chart.FirstVisibleBarIndex) / 2, Chart.TopY - priceToCover.Value,
                        Chart.LastVisibleBarIndex - barsToCover.Value, Chart.BottomY + priceToCover.Value, _drawingColor);
                    break;

                case ChartObjectType.FibonacciRetracement:
                    chartObject = Chart.DrawFibonacciRetracement(GetObjectName(type), Chart.FirstVisibleBarIndex + barsToCover.Value, Chart.BottomY + priceToCover.Value, Chart.LastVisibleBarIndex - barsToCover.Value, Chart.TopY - priceToCover.Value, _drawingColor);
                    break;

                case ChartObjectType.FibonacciFan:
                    chartObject = Chart.DrawFibonacciFan(GetObjectName(type), Chart.FirstVisibleBarIndex + barsToCover.Value, Chart.BottomY + priceToCover.Value, Chart.LastVisibleBarIndex - barsToCover.Value, Chart.TopY - priceToCover.Value, _drawingColor);
                    break;

                case ChartObjectType.FibonacciExpansion:
                    chartObject = Chart.DrawFibonacciExpansion(GetObjectName(type), Chart.FirstVisibleBarIndex + barsToCover.Value, Chart.BottomY + priceToCover.Value, Chart.FirstVisibleBarIndex + (Chart.LastVisibleBarIndex - Chart.FirstVisibleBarIndex) / 2, Chart.TopY - priceToCover.Value,
                        Chart.LastVisibleBarIndex - barsToCover.Value, Chart.BottomY + priceToCover.Value, _drawingColor); break;
            }

            if (chartObject != null)
            {
                chartObject.IsInteractive = true;

                var chartShape = chartObject as ChartShape;

                if (chartShape != null)
                {
                    chartShape.IsFilled = true;
                }
            }
        }

        private void CancelOrders(PendingOrderType orderType)
        {
            var botOrders = PendingOrders.ToArray();

            foreach (var order in botOrders)
            {
                if (order.Label.Equals(Label, StringComparison.OrdinalIgnoreCase) == false || order.OrderType != orderType) continue;

                CancelPendingOrder(order);
            }
        }

        private void CloseMarketOrders()
        {
            var botPositions = Positions.FindAll(Label);

            foreach (var position in botPositions)
            {
                ClosePositionAsync(position);
            }
        }

        private void PlaceOrder(PendingOrderType orderType, TradeType tradeType)
        {
            double targetPrice;

            switch (orderType)
            {
                case PendingOrderType.Limit:
                    targetPrice = tradeType == TradeType.Buy ? Symbol.Bid - PendingOrderDistanceInPips : Symbol.Bid + PendingOrderDistanceInPips;

                    PlaceLimitOrderAsync(tradeType, SymbolName, Volume, targetPrice, Label, _stopLossInPips, _takeProfitInPips);
                    break;

                case PendingOrderType.Stop:
                    targetPrice = tradeType == TradeType.Buy ? Symbol.Bid + PendingOrderDistanceInPips : Symbol.Bid - PendingOrderDistanceInPips;

                    PlaceStopOrderAsync(tradeType, SymbolName, Volume, targetPrice, Label, _stopLossInPips, _takeProfitInPips);
                    break;

                case PendingOrderType.StopLimit:
                    targetPrice = tradeType == TradeType.Buy ? Symbol.Bid + PendingOrderDistanceInPips : Symbol.Bid - PendingOrderDistanceInPips;

                    PlaceStopLimitOrderAsync(tradeType, SymbolName, Volume, targetPrice, StopLimitRangeInPips, Label, _stopLossInPips, _takeProfitInPips);
                    break;
            }
        }

        private void MarketOrderKeyHandler(TradeType tradeType)
        {
            ExecuteMarketOrderAsync(tradeType, SymbolName, Volume, Label, _stopLossInPips, _takeProfitInPips);
        }

        private Color GetColor(string colorString, int alpha = 255)
        {
            var color = colorString[0] == '#' ? Color.FromHex(colorString) : Color.FromName(colorString);

            return Color.FromArgb(alpha, color);
        }

        private string GetHotkeyText(Key key, ModifierKeys modifier)
        {
            return modifier == ModifierKeys.None ? key.ToString() : string.Format("{0}+{1}", modifier, key);
        }

        private string GetObjectName(ChartObjectType type)
        {
            return string.Format("{0}_{1}_{2}", _drawingObjectsNamePrefix, type, DateTimeOffset.Now.Ticks);
        }
    }
}
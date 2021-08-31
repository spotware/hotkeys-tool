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

        [Parameter("Horizontal Alignment", DefaultValue = HorizontalAlignment.Right, Group = "Chart Controls")]
        public HorizontalAlignment HorizontalAlignment { get; set; }

        [Parameter("Vertical Alignment", DefaultValue = VerticalAlignment.Top, Group = "Chart Controls")]
        public VerticalAlignment VerticalAlignment { get; set; }

        [Parameter("Background Color", DefaultValue = "Yellow", Group = "Chart Controls")]
        public string BackgroundColor { get; set; }

        [Parameter("Text Color", DefaultValue = "Black", Group = "Chart Controls")]
        public string TextColor { get; set; }

        [Parameter("Opacity", DefaultValue = 0.6, MinValue = 0, MaxValue = 1, Group = "Chart Controls")]
        public double Opacity { get; set; }

        [Parameter("Margin", DefaultValue = 5, MinValue = 0, Group = "Chart Controls")]
        public double Margin { get; set; }

        [Parameter("Font Size", DefaultValue = 14, MinValue = 0, Group = "Chart Controls")]
        public double FontSize { get; set; }

        [Parameter("Font Weight", DefaultValue = FontWeight.Normal, Group = "Chart Controls")]
        public FontWeight FontWeight { get; set; }

        protected override void OnStart()
        {
            if (StopLossInPips > 0) _stopLossInPips = StopLossInPips;
            if (TakeProfitInPips > 0) _takeProfitInPips = TakeProfitInPips;

            PendingOrderDistanceInPips *= Symbol.PipSize;

            AddTradingHotkeys();

            ShowHotkeysOnChart();
        }

        private void ShowHotkeysOnChart()
        {
            var grid = new Grid(13, 2)
            {
                HorizontalAlignment = HorizontalAlignment,
                VerticalAlignment = VerticalAlignment,
                Opacity = Opacity,
                BackgroundColor = GetColor(BackgroundColor)
            };

            var textBlocksStyle = new Style();

            textBlocksStyle.Set(ControlProperty.Margin, Margin);
            textBlocksStyle.Set(ControlProperty.FontSize, FontSize);
            textBlocksStyle.Set(ControlProperty.FontWeight, FontWeight);
            textBlocksStyle.Set(ControlProperty.ForegroundColor, GetColor(TextColor));

            var tradingHotkeysTextblock = new TextBlock { Text = "Trading", HorizontalAlignment = HorizontalAlignment.Center, Style = textBlocksStyle };

            grid.AddChild(tradingHotkeysTextblock, 0, 0, 1, 2);

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

            Chart.AddControl(grid);
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

        private void CancelOrders(PendingOrderType orderType)
        {
            var botOrders = PendingOrders.ToArray();

            foreach (var order in botOrders)
            {
                if (order.OrderType != orderType) continue;

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
            return modifier == ModifierKeys.None ? key.ToString() : string.Format("{0}+{1}", key, modifier);
        }
    }
}
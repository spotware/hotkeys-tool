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

        protected override void OnStart()
        {
            if (StopLossInPips > 0) _stopLossInPips = StopLossInPips;
            if (TakeProfitInPips > 0) _takeProfitInPips = TakeProfitInPips;

            PendingOrderDistanceInPips *= Symbol.PipSize;

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
    }
}

# ArkhamTransmitter 
 A self-hosted webhook relay that forwards Arkham Intelligence whale alerts directly to Telegram


## Problem

[Arkham Intelligence](https://arkhamintelligence.com) is one of the best on chain whale tracking tools available. As a crypto trader, tracking large wallet movements gives a real advantage. For example, knowing when blackrock moves $100m BTC can decide your next setup or early exit on running setups. 

The problem: Somehow Telegram blocked Arkham's direct alert delivery. No one knows exactly why, but the result is the same Arkham can't push notifications straight to your Telegram bot.

The obvious workaround was Zapier connect Arkham webhook → Zapier → Telegram. It worked. For 7 days. Then Zapier asked for $29/month for just 100 notifications. Now imagine you receive 5000 notifications a month lol. nahhhh

That's why ArkhamTransmitter was born.



## What It Does

ArkhamTransmitter is a lightweight ASP.NET Core Minimal API that:

1. **Receives webhook payloads** from Arkham Intelligence 
2. **Parses the alert data** (wallet address, transaction amount, asset type, etc.)
3. **Forwards the notification** to your Telegram bot instantly

## Planned Updates
1. **Planned Updates (TODO)** (discord, whatsapp, email, slack)
2. **Automatic Execution** Integrate with crypto exchange APIs to allow real-world copy trading based on configured alert triggers. 
   

 No fxxx subscription fee!!!

------------------------------------------------------------------------------------

## Getting Started

### Prerequisites
- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0) or Docker
- A Telegram Bot token (create via [@BotFather](https://t.me/BotFather))
- Your Telegram Chat ID
- An Arkham Intelligence account with webhook alerts configured. (Can also used prefered services not just arkham)

### Configuration

Set the following environment variables:

```bash
TELEGRAM_BOT_TOKEN=your_bot_token_here
TELEGRAM_CHAT_ID=your_chat_id_here
```

### Run with Docker

```bash
docker build -t arkham-transmitter .
docker run -d \
  -p 8080:8080 \
  -e TELEGRAM_BOT_TOKEN=your_token \
  -e TELEGRAM_CHAT_ID=your_chat_id \
  arkham-transmitter
```

### Run locally

```bash
git clone https://github.com/beez6239/ArkhamTransmitter.git
cd ArkhamTransmitter/Alerter
dotnet restore
dotnet run
```

### Configure Arkham Webhook

In your Arkham Intelligence dashboard, set your webhook URL to:
```
https://yourendpoint.com/webhook

```

## Deployment (Docker )
Deploy to any VPS, cloud VM, or container platform (Railway, Fly.io, Render  all work with Docker)

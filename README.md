# AI Customer Feedback Analytics
![.NET 8](https://img.shields.io/badge/.NET-8-purple)
![RabbitMQ](https://img.shields.io/badge/RabbitMQ-Message_Broker-orange)
![Azure OpenAI](https://img.shields.io/badge/Azure_OpenAI-GPT--4o-blue)
![Docker](https://img.shields.io/badge/Docker-Containerized-blue)
![License](https://img.shields.io/badge/License-MIT-green)

Analysis Service consumes FeedbackSubmitted events, performs sentiment analysis and embedding generation with Azure OpenAI, stores results in PostgreSQL, indexes analyses into Azure AI Search, and runs hybrid similarity search (vector + keyword) to find the top 5 related feedback items. It exposes filter-based analysis retrieval APIs and integrates with RabbitMQ via MassTransit.

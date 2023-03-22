# Azure OpenAI App Innovation Workshop 

## Enterprise GPT Intelligent Application Architecture
The importance of Prompt Engineering in the field of large-scale language model applications is gradually becoming apparent. Prompt Engineering is a technique based on human language intuition that can help enterprises quickly achieve specific functions when building large language models. Based on the formal representation of natural language questions, Prompt Engineering can effectively guide the learning process of large language models, thereby improving their performance. Some papers have shown that in certain complex reasoning and thought-chain contexts, prompts even perform better than fine-tuning.

Based on these research findings, we have proposed a framework for enterprise-level GPT intelligent application engineering, which is based on Prompt Engineering. The framework, as shown in the diagram below, allows enterprises to quickly build their own large-scale intelligent applications without modifying the model itself, and immediately enjoy the productivity dividend brought by the new generation of artificial intelligence.

 ![architecture](./media/Enterprise-GPT-Intelligent-App-Workshop.jpg)

As shown in the diagram, this architecture utilizes a flexible prompt engine and supports various deployment forms such as PaaS, Serverless, and containers, enabling enterprises to optimize the prompt engine using their existing development technology stack and elastically scale computing resources according to real-time business needs. The session and token data service layer, based on Redis and CosmosDB, adds contextual caching, session persistence, prompt persistence, and other capabilities to the application, leaving room for future model or engine optimization based on prompts. The API encapsulation, load balancing, and gateway at the front end further enhance application security and reliability, allowing the intelligent entity to integrate with various front-end apps in a more secure and stable manner. Most importantly, this architecture is designed with a dual-engine architecture based on Azure Cognitive Search and Embedding, two additional private knowledge base technologies. On the one hand, Azure Cognitive Search can quickly index unstructured data such as PDF and WORD files, allowing existing data to be used immediately. On the other hand, by utilizing Azure PostgreSQL's vector storage and processing capabilities and combining them with Azure OpenAI's embedding vector generation model, the enterprise's existing structured knowledge base can be integrated with the prompt engine, producing more accurate, stable, and reliable results from the GPT model.

## Azure OpenAI App Innovation In-A-Day Workshop 

This is a full-day workshop focused on exploring the use of Azure PaaS and OpenAI services to build modern intelligent applications. It aims to provide practical skills and experience in developing innovative intelligent applications on Azure for IT and architecture decision-makers, business decision-makers, and developers.

In this workshop, attendees will learn how to select Azure workloads to design a modern GPT intelligent application, understand Azure OpenAI services, explore the capabilities of large model prompt engineering, understand common parameters and options in the OpenAI API, and land an Enterprise GPT intelligent application architecture inside their organization through hands-on experimentation.

Whether you are a novice or an experienced professional, this workshop will meet the expectations and needs of participants. Attendees will be able to interact with experts, share experiences, and gain inspiration and insights from real project cases. Together, we will explore exciting GPT intelligent applications and usher in a new era of intelligent applications!

- Workshop content [Chinese](./Workshop%20Content%20CHS/)
- Workshop content [English](./Workshop%20Content%20EN/)
- Demo notebook [here](./Demo%20Notebook/)
- Source code [here](./Source%20Code/)
- Hands on lab manual [here](./Hands%20on%20lab%20manual/)

### Product Documents

- [Azure Openai Doc](https://learn.microsoft.com/zh-cn/azure/cognitive-services/openai/)
- [Azure PaaS Doc](https://learn.microsoft.com/zh-cn/azure/?product=web)
- [Azure Data Service Doc](https://learn.microsoft.com/zh-cn/azure/?product=databases)
- [Azure Cognitive Search Docu](https://learn.microsoft.com/zh-cn/azure/search/)

### Reference
- [How does GPT obtain its ability](https://yaofu.notion.site/How-does-GPT-Obtain-its-Ability-Tracing-Emergent-Abilities-of-Language-Models-to-their-Sources-b9a57ac0fcf74f30a1ab9e3e36fa1dc1#a83aa8c34a254289ace924fa83e0b9c9)
- [Jason Wei, Xuezhi Wang, Dale Schuurmans, et al. Chain-of-Thought Prompting Elicits Reasoning in Large Language Models. arXiv, 2022](https://arxiv.org/abs/2201.11903)
- [Best practices for prompt engineering with openai api](https://help.openai.com/en/articles/6654000-best-practices-for-prompt-engineering-with-openai-api)
- [Awesome ChatGPT Prompts](https://github.com/f/awesome-chatgpt-prompts/)


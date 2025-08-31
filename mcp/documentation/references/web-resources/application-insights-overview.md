# Performance Tool Documentation

**Source:** https://learn.microsoft.com/en-us/azure/azure-monitor/app/app-insights-overview
**Downloaded:** 08/30/2025 17:27:15

---

 <!DOCTYPE html>
		<html
			class="layout layout-holy-grail   show-table-of-contents conceptual show-breadcrumb default-focus"
			lang="en-us"
			dir="ltr"
			data-authenticated="false"
			data-auth-status-determined="false"
			data-target="docs"
			x-ms-format-detection="none"
		>
			
		<head>
			<title>Application Insights OpenTelemetry observability overview - Azure Monitor | Microsoft Learn</title>
			<meta charset="utf-8" />
			<meta name="viewport" content="width=device-width, initial-scale=1.0" />
			<meta name="color-scheme" content="light dark" />

			<meta name="description" content="Learn how Azure Monitor Application Insights integrates with OpenTelemetry (OTel) for comprehensive application observability." />
			<link rel="canonical" href="https://learn.microsoft.com/en-us/azure/azure-monitor/app/app-insights-overview" /> 

			<!-- Non-customizable open graph and sharing-related metadata -->
			<meta name="twitter:card" content="summary_large_image" />
			<meta name="twitter:site" content="@MicrosoftLearn" />
			<meta property="og:type" content="website" />
			<meta property="og:image:alt" content="Microsoft Learn" />
			<meta property="og:image" content="https://learn.microsoft.com/en-us/media/open-graph-image.png" />
			<!-- Page specific open graph and sharing-related metadata -->
			<meta property="og:title" content="Application Insights OpenTelemetry observability overview - Azure Monitor" />
			<meta property="og:url" content="https://learn.microsoft.com/en-us/azure/azure-monitor/app/app-insights-overview" />
			<meta property="og:description" content="Learn how Azure Monitor Application Insights integrates with OpenTelemetry (OTel) for comprehensive application observability." />
			<meta name="platform_id" content="1be2d078-71c2-a785-111a-b0ba14c63781" /> <meta name="scope" content="Azure" /><meta name="scope" content="Azure Monitor" />
			<meta name="locale" content="en-us" />
			 <meta name="adobe-target" content="true" />
			<meta name="uhfHeaderId" content="azure" />

			<meta name="page_type" content="conceptual" />

			<!--page specific meta tags-->
			

			<!-- custom meta tags -->
			
		<meta name="breadcrumb_path" content="../../breadcrumb/azure-monitor/toc.json" />
	
		<meta name="feedback_help_link_url" content="https://learn.microsoft.com/answers/tags/20/azure-monitor/" />
	
		<meta name="feedback_help_link_type" content="get-help-at-qna" />
	
		<meta name="feedback_product_url" content="https://feedback.azure.com/d365community/forum/3887dc70-2025-ec11-b6e6-000d3a4f09d0" />
	
		<meta name="feedback_system" content="Standard" />
	
		<meta name="permissioned-type" content="public" />
	
		<meta name="recommendations" content="true" />
	
		<meta name="recommendation_types" content="Training" />
	
		<meta name="recommendation_types" content="Certification" />
	
		<meta name="ms.suite" content="office" />
	
		<meta name="author" content="AarDavMax" />
	
		<meta name="learn_banner_products" content="azure" />
	
		<meta name="manager" content="eliotgra" />
	
		<meta name="ms.author" content="aaronmax" />
	
		<meta name="ms.service" content="azure-monitor" />
	
		<meta name="ms.subservice" content="application-insights" />
	
		<meta name="ms.topic" content="overview" />
	
		<meta name="ms.date" content="2025-09-25T00:00:00Z" />
	
		<meta name="document_id" content="0051106b-78fc-6ad3-b7ca-2cc6f87f1821" />
	
		<meta name="document_version_independent_id" content="72c16b2c-9ead-85dd-78b5-11c9b1635211" />
	
		<meta name="updated_at" content="2025-08-26T22:16:00Z" />
	
		<meta name="original_content_git_url" content="https://github.com/MicrosoftDocs/azure-monitor-docs-pr/blob/live/articles/azure-monitor/app/app-insights-overview.md" />
	
		<meta name="gitcommit" content="https://github.com/MicrosoftDocs/azure-monitor-docs-pr/blob/ae163d8d1ec10145bb4f562c229a9e61041ed847/articles/azure-monitor/app/app-insights-overview.md" />
	
		<meta name="git_commit_id" content="ae163d8d1ec10145bb4f562c229a9e61041ed847" />
	
		<meta name="site_name" content="Docs" />
	
		<meta name="depot_name" content="Learn.azure-monitor" />
	
		<meta name="schema" content="Conceptual" />
	
		<meta name="toc_rel" content="../toc.json" />
	
		<meta name="word_count" content="572" />
	
		<meta name="asset_id" content="azure-monitor/app/app-insights-overview" />
	
		<meta name="moniker_range_name" content="" />
	
		<meta name="item_type" content="Content" />
	
		<meta name="source_path" content="articles/azure-monitor/app/app-insights-overview.md" />
	
		<meta name="previous_tlsh_hash" content="7D1FD691540DDE30EFB2224B9BD5DA112AF0C0C8BCB02AC5113555D3890D6E33AEEDBA69EBDBE74125C66BD330F7259686C1AB7B953C3AC59E25006D921C528253C937F368" />
	
		<meta name="github_feedback_content_git_url" content="https://github.com/MicrosoftDocs/azure-monitor-docs/blob/main/articles/azure-monitor/app/app-insights-overview.md" />
	 
		<meta name="cmProducts" content="https://authoring-docs-microsoft.poolparty.biz/devrel/07bb3e10-d135-43ff-bc8b-360497cb39fa" data-source="generated" />
	
		<meta name="spProducts" content="https://authoring-docs-microsoft.poolparty.biz/devrel/12e559b9-eaf6-4aee-9af7-62334e15f863" data-source="generated" />
	

			<!-- assets and js globals -->
			
			<link rel="stylesheet" href="/static/assets/0.4.03160.7077-e1cebf9d/styles/site-ltr.css" />
			<link rel="preconnect" href="//mscom.demdex.net" crossorigin />
						<link rel="dns-prefetch" href="//target.microsoft.com" />
						<link rel="dns-prefetch" href="//microsoftmscompoc.tt.omtrdc.net" />
						<link
							rel="preload"
							as="script"
							href="/static/third-party/adobe-target/at-js/2.9.0/at.js"
							integrity="sha384-1/viVM50hgc33O2gOgkWz3EjiD/Fy/ld1dKYXJRUyjNYVEjSUGcSN+iPiQF7e4cu"
							crossorigin="anonymous"
							id="adobe-target-script"
							type="application/javascript"
						/>
			<script src="https://wcpstatic.microsoft.com/mscc/lib/v2/wcp-consent.js"></script>
			<script src="https://js.monitor.azure.com/scripts/c/ms.jsll-4.min.js"></script>
			<script src="/_themes/docs.theme/master/en-us/_themes/global/deprecation.js"></script>

			<!-- msdocs global object -->
			<script id="msdocs-script">
		var msDocs = {
  "environment": {
    "accessLevel": "online",
    "azurePortalHostname": "portal.azure.com",
    "reviewFeatures": false,
    "supportLevel": "production",
    "systemContent": true,
    "siteName": "learn",
    "legacyHosting": false
  },
  "data": {
    "contentLocale": "en-us",
    "contentDir": "ltr",
    "userLocale": "en-us",
    "userDir": "ltr",
    "pageTemplate": "Conceptual",
    "brand": "azure",
    "context": {},
    "standardFeedback": true,
    "showFeedbackReport": false,
    "feedbackHelpLinkType": "get-help-at-qna",
    "feedbackHelpLinkUrl": "https://learn.microsoft.com/answers/tags/20/azure-monitor/",
    "feedbackSystem": "Standard",
    "feedbackGitHubRepo": "MicrosoftDocs/azure-docs",
    "feedbackProductUrl": "https://feedback.azure.com/d365community/forum/3887dc70-2025-ec11-b6e6-000d3a4f09d0",
    "extendBreadcrumb": false,
    "isEditDisplayable": true,
    "isPrivateUnauthorized": false,
    "hideViewSource": false,
    "isPermissioned": false,
    "hasRecommendations": true,
    "contributors": [
      {
        "name": "AarDavMax",
        "url": "https://github.com/AarDavMax"
      },
      {
        "name": "kainawroth",
        "url": "https://github.com/kainawroth"
      },
      {
        "name": "CourtGoodson",
        "url": "https://github.com/CourtGoodson"
      },
      {
        "name": "TimoSalomaki",
        "url": "https://github.com/TimoSalomaki"
      },
      {
        "name": "cijothomas",
        "url": "https://github.com/cijothomas"
      },
      {
        "name": "DaleKoetke",
        "url": "https://github.com/DaleKoetke"
      },
      {
        "name": "KennedyDenMSFT",
        "url": "https://github.com/KennedyDenMSFT"
      },
      {
        "name": "KarlErickson",
        "url": "https://github.com/KarlErickson"
      },
      {
        "name": "v-jbasden",
        "url": "https://github.com/v-jbasden"
      },
      {
        "name": "prmerger-automator[bot]",
        "url": "https://github.com/prmerger-automator[bot]"
      },
      {
        "name": "rboucher",
        "url": "https://github.com/rboucher"
      },
      {
        "name": "mattmccleary",
        "url": "https://github.com/mattmccleary"
      },
      {
        "name": "pritamso",
        "url": "https://github.com/pritamso"
      },
      {
        "name": "jake-fawcett",
        "url": "https://github.com/jake-fawcett"
      },
      {
        "name": "SwathiDhanwada-MSFT",
        "url": "https://github.com/SwathiDhanwada-MSFT"
      },
      {
        "name": "trask",
        "url": "https://github.com/trask"
      },
      {
        "name": "abinetabate",
        "url": "https://github.com/abinetabate"
      },
      {
        "name": "19BMG00",
        "url": "https://github.com/19BMG00"
      },
      {
        "name": "bwren",
        "url": "https://github.com/bwren"
      },
      {
        "name": "v-chmccl",
        "url": "https://github.com/v-chmccl"
      },
      {
        "name": "lgayhardt",
        "url": "https://github.com/lgayhardt"
      },
      {
        "name": "thebitstreamer",
        "url": "https://github.com/thebitstreamer"
      },
      {
        "name": "v-hearya",
        "url": "https://github.com/v-hearya"
      },
      {
        "name": "jdmartinez36",
        "url": "https://github.com/jdmartinez36"
      },
      {
        "name": "DCtheGeek",
        "url": "https://github.com/DCtheGeek"
      },
      {
        "name": "v-kents",
        "url": "https://github.com/v-kents"
      },
      {
        "name": "mrbullwinkle",
        "url": "https://github.com/mrbullwinkle"
      },
      {
        "name": "lzchen",
        "url": "https://github.com/lzchen"
      },
      {
        "name": "markwragg",
        "url": "https://github.com/markwragg"
      },
      {
        "name": "theheatDK",
        "url": "https://github.com/theheatDK"
      }
    ]
  },
  "functions": {}
};;
	</script>

			<!-- base scripts, msdocs global should be before this -->
			<script src="/static/assets/0.4.03160.7077-e1cebf9d/scripts/en-us/index-docs.js"></script>
			

			<!-- json-ld -->
			
		</head>
	
			<body
				id="body"
				data-bi-name="body"
				class="layout-body "
				lang="en-us"
				dir="ltr"
			>
				<header class="layout-body-header">
		<div class="header-holder has-default-focus">
			
		<a
			href="#main"
			
			style="z-index: 1070"
			class="outline-color-text visually-hidden-until-focused position-fixed inner-focus focus-visible top-0 left-0 right-0 padding-xs text-align-center background-color-body"
			
		>
			Skip to main content
		</a>
	
		<a
			href="#"
			data-skip-to-ask-learn
			style="z-index: 1070"
			class="outline-color-text visually-hidden-until-focused position-fixed inner-focus focus-visible top-0 left-0 right-0 padding-xs text-align-center background-color-body"
			hidden
		>
			Skip to Ask Learn chat experience
		</a>
	

			<div hidden id="cookie-consent-holder" data-test-id="cookie-consent-container"></div>
			<!-- Unsupported browser warning -->
			<div
				id="unsupported-browser"
				style="background-color: white; color: black; padding: 16px; border-bottom: 1px solid grey;"
				hidden
			>
				<div style="max-width: 800px; margin: 0 auto;">
					<p style="font-size: 24px">This browser is no longer supported.</p>
					<p style="font-size: 16px; margin-top: 16px;">
						Upgrade to Microsoft Edge to take advantage of the latest features, security updates, and technical support.
					</p>
					<div style="margin-top: 12px;">
						<a
							href="https://go.microsoft.com/fwlink/p/?LinkID=2092881 "
							style="background-color: #0078d4; border: 1px solid #0078d4; color: white; padding: 6px 12px; border-radius: 2px; display: inline-block;"
						>
							Download Microsoft Edge
						</a>
						<a
							href="https://learn.microsoft.com/en-us/lifecycle/faq/internet-explorer-microsoft-edge"
							style="background-color: white; padding: 6px 12px; border: 1px solid #505050; color: #171717; border-radius: 2px; display: inline-block;"
						>
							More info about Internet Explorer and Microsoft Edge
						</a>
					</div>
				</div>
			</div>
			<!-- site header -->
			<header
				id="ms--site-header"
				data-test-id="site-header-wrapper"
				role="banner"
				itemscope="itemscope"
				itemtype="http://schema.org/Organization"
			>
				<div
					id="ms--mobile-nav"
					class="site-header display-none-tablet padding-inline-none gap-none"
					data-bi-name="mobile-header"
					data-test-id="mobile-header"
				></div>
				<div
					id="ms--primary-nav"
					class="site-header display-none display-flex-tablet"
					data-bi-name="L1-header"
					data-test-id="primary-header"
				></div>
				<div
					id="ms--secondary-nav"
					class="site-header display-none display-flex-tablet"
					data-bi-name="L2-header"
					data-test-id="secondary-header"
				></div>
			</header>
			
		<!-- banner -->
		<div data-banner>
			<div id="disclaimer-holder"></div>
			
		</div>
		<!-- banner end -->
	
		</div>
	</header>
				 <section
					id="layout-body-menu"
					class="layout-body-menu display-flex"
					data-bi-name="menu"
			  >
					<div
		id="left-container"
		class="left-container display-none display-block-tablet padding-inline-sm padding-bottom-sm width-full"
	>
		<nav
			id="affixed-left-container"
			class="margin-top-sm-tablet position-sticky display-flex flex-direction-column"
			aria-label="Primary"
		></nav>
	</div>
			  </section>

				<main
					id="main"
					role="main"
					class="layout-body-main "
					data-bi-name="content"
					lang="en-us"
					dir="ltr"
				>
					
			<div
		id="ms--content-header"
		class="content-header default-focus border-bottom-none"
		data-bi-name="content-header"
	>
		<div class="content-header-controls margin-xxs margin-inline-sm-tablet">
			<button
				type="button"
				class="contents-button button button-sm margin-right-xxs"
				data-bi-name="contents-expand"
				aria-haspopup="true"
				data-contents-button
			>
				<span class="icon" aria-hidden="true"><span class="docon docon-menu"></span></span>
				<span class="contents-expand-title"> Table of contents </span>
			</button>
			<button
				type="button"
				class="ap-collapse-behavior ap-expanded button button-sm"
				data-bi-name="ap-collapse"
				aria-controls="action-panel"
			>
				<span class="icon" aria-hidden="true"><span class="docon docon-exit-mode"></span></span>
				<span>Exit editor mode</span>
			</button>
		</div>
	</div>
			<div data-main-column class="padding-sm padding-top-none padding-top-sm-tablet">
				<div>
					
		<div id="article-header" class="background-color-body margin-bottom-xs display-none-print">
			<div class="display-flex align-items-center justify-content-space-between">
				
		<details
			id="article-header-breadcrumbs-overflow-popover"
			class="popover"
			data-for="article-header-breadcrumbs"
		>
			<summary
				class="button button-clear button-primary button-sm inner-focus"
				aria-label="All breadcrumbs"
			>
				<span class="icon">
					<span class="docon docon-more"></span>
				</span>
			</summary>
			<div id="article-header-breadcrumbs-overflow" class="popover-content padding-none"></div>
		</details>

		<bread-crumbs
			id="article-header-breadcrumbs"
			data-test-id="article-header-breadcrumbs"
			class="overflow-hidden flex-grow-1 margin-right-sm margin-right-md-tablet margin-right-lg-desktop margin-left-negative-xxs padding-left-xxs"
		></bread-crumbs>
	 
		<div
			id="article-header-page-actions"
			class="opacity-none margin-left-auto display-flex flex-wrap-no-wrap align-items-stretch"
		>
			
		<button
			class="button button-sm border-none inner-focus display-none-tablet flex-shrink-0 "
			data-bi-name="ask-learn-assistant-entry"
			data-test-id="ask-learn-assistant-modal-entry-mobile"
			data-ask-learn-modal-entry
			type="button"
			style="min-width: max-content;"
			aria-expanded="false"
			aria-label="Ask Learn"
			hidden
		>
			<span class="icon font-size-lg" aria-hidden="true">
				<span class="docon docon-chat-sparkle-fill gradient-ask-learn-logo"></span>
			</span>
		</button>
		<button
			class="button button-sm display-none display-inline-flex-tablet display-none-desktop flex-shrink-0 margin-right-xxs border-color-ask-learn "
			data-bi-name="ask-learn-assistant-entry"
			data-test-id="ask-learn-assistant-modal-entry-tablet"
			data-ask-learn-modal-entry
			type="button"
			style="min-width: max-content;"
			aria-expanded="false"
			hidden
		>
			<span class="icon font-size-lg" aria-hidden="true">
				<span class="docon docon-chat-sparkle-fill gradient-ask-learn-logo"></span>
			</span>
			<span>Ask Learn</span>
		</button>
		<button
			class="button button-sm display-none flex-shrink-0 display-inline-flex-desktop margin-right-xxs	border-color-ask-learn "
			data-bi-name="ask-learn-assistant-entry"
			data-test-id="ask-learn-assistant-flyout-entry"
			data-ask-learn-flyout-entry
			data-flyout-button="toggle"
			type="button"
			style="min-width: max-content;"
			aria-expanded="false"
			aria-controls="ask-learn-flyout"
			hidden
		>
			<span class="icon font-size-lg" aria-hidden="true">
				<span class="docon docon-chat-sparkle-fill gradient-ask-learn-logo"></span>
			</span>
			<span>Ask Learn</span>
		</button>
	 
		<button
			type="button"
			id="ms--focus-mode-button"
			data-focus-mode
			data-bi-name="focus-mode-entry"
			class="button button-sm flex-shrink-0 margin-right-xxs display-none display-inline-flex-desktop"
		>
			<span class="icon font-size-lg" aria-hidden="true">
				<span class="docon docon-glasses"></span>
			</span>
			<span>Focus mode</span>
		</button>
	 

			<details class="popover popover-right" id="article-header-page-actions-overflow">
				<summary
					class="justify-content-flex-start button button-clear button-sm button-primary inner-focus"
					aria-label="More actions"
					title="More actions"
				>
					<span class="icon" aria-hidden="true">
						<span class="docon docon-more-vertical"></span>
					</span>
				</summary>
				<div class="popover-content">
					
		<button
			data-page-action-item="overflow-mobile"
			type="button"
			class="button-block button-sm has-inner-focus button button-clear display-none-tablet justify-content-flex-start text-align-left"
			data-bi-name="contents-expand"
			data-contents-button
			data-popover-close
		>
			<span class="icon">
				<span class="docon docon-editor-list-bullet" aria-hidden="true"></span>
			</span>
			<span class="contents-expand-title">Table of contents</span>
		</button>
	 
		<a
			id="lang-link-overflow"
			class="button-sm has-inner-focus button button-clear button-block justify-content-flex-start text-align-left"
			data-bi-name="language-toggle"
			data-page-action-item="overflow-all"
			data-check-hidden="true"
			data-read-in-link
			href="#"
			hidden
		>
			<span class="icon" aria-hidden="true" data-read-in-link-icon>
				<span class="docon docon-locale-globe"></span>
			</span>
			<span data-read-in-link-text>Read in English</span>
		</a>
	 
		<button
			type="button"
			class="collection button button-clear button-sm button-block justify-content-flex-start text-align-left inner-focus"
			data-list-type="collection"
			data-bi-name="collection"
			data-page-action-item="overflow-all"
			data-check-hidden="true"
			data-popover-close
		>
			<span class="icon" aria-hidden="true">
				<span class="docon docon-circle-addition"></span>
			</span>
			<span class="collection-status">Add</span>
		</button>
	
					
		<button
			type="button"
			class="collection button button-block button-clear button-sm justify-content-flex-start text-align-left inner-focus"
			data-list-type="plan"
			data-bi-name="plan"
			data-page-action-item="overflow-all"
			data-check-hidden="true"
			data-popover-close
			hidden
		>
			<span class="icon" aria-hidden="true">
				<span class="docon docon-circle-addition"></span>
			</span>
			<span class="plan-status">Add to plan</span>
		</button>
	  
		<a
			data-contenteditbtn
			class="button button-clear button-block button-sm inner-focus justify-content-flex-start text-align-left text-decoration-none"
			data-bi-name="edit"
			
			href="https://github.com/MicrosoftDocs/azure-monitor-docs/blob/main/articles/azure-monitor/app/app-insights-overview.md"
			data-original_content_git_url="https://github.com/MicrosoftDocs/azure-monitor-docs-pr/blob/live/articles/azure-monitor/app/app-insights-overview.md"
			data-original_content_git_url_template="{repo}/blob/{branch}/articles/azure-monitor/app/app-insights-overview.md"
			data-pr_repo=""
			data-pr_branch=""
		>
			<span class="icon" aria-hidden="true">
				<span class="docon docon-edit-outline"></span>
			</span>
			<span>Edit</span>
		</a>
	
					
		<hr class="margin-block-xxs" />
		<h4 class="font-size-sm padding-left-xxs">Share via</h4>
		
					<a
						class="button button-clear button-sm inner-focus button-block justify-content-flex-start text-align-left text-decoration-none share-facebook"
						data-bi-name="facebook"
						data-page-action-item="overflow-all"
						href="#"
					>
						<span class="icon color-primary" aria-hidden="true">
							<span class="docon docon-facebook-share"></span>
						</span>
						<span>Facebook</span>
					</a>

					<a
						href="#"
						class="button button-clear button-sm inner-focus button-block justify-content-flex-start text-align-left text-decoration-none share-twitter"
						data-bi-name="twitter"
						data-page-action-item="overflow-all"
					>
						<span class="icon color-text" aria-hidden="true">
							<span class="docon docon-xlogo-share"></span>
						</span>
						<span>x.com</span>
					</a>

					<a
						href="#"
						class="button button-clear button-sm inner-focus button-block justify-content-flex-start text-align-left text-decoration-none share-linkedin"
						data-bi-name="linkedin"
						data-page-action-item="overflow-all"
					>
						<span class="icon color-primary" aria-hidden="true">
							<span class="docon docon-linked-in-logo"></span>
						</span>
						<span>LinkedIn</span>
					</a>
					<a
						href="#"
						class="button button-clear button-sm inner-focus button-block justify-content-flex-start text-align-left text-decoration-none share-email"
						data-bi-name="email"
						data-page-action-item="overflow-all"
					>
						<span class="icon color-primary" aria-hidden="true">
							<span class="docon docon-mail-message"></span>
						</span>
						<span>Email</span>
					</a>
			  
	 
		<hr class="margin-block-xxs" />
		<button
			class="button button-block button-clear button-sm justify-content-flex-start text-align-left inner-focus"
			type="button"
			data-bi-name="print"
			data-page-action-item="overflow-all"
			data-popover-close
			data-print-page
			data-check-hidden="true"
		>
			<span class="icon color-primary" aria-hidden="true">
				<span class="docon docon-print"></span>
			</span>
			<span>Print</span>
		</button>
	
				</div>
			</details>
		</div>
	
			</div>
		</div>
	
					<!-- azure disclaimer -->
					
					<!-- privateUnauthorizedTemplate is hidden by default -->
					
		<div unauthorized-private-section data-bi-name="permission-content-unauthorized-private" hidden>
			<hr class="hr margin-top-xs margin-bottom-sm" />
			<div class="notification notification-info">
				<div class="notification-content">
					<p class="margin-top-none notification-title">
						<span class="icon">
							<span class="docon docon-exclamation-circle-solid" aria-hidden="true"></span>
						</span>
						<span>Note</span>
					</p>
					<p class="margin-top-none authentication-determined not-authenticated">
						Access to this page requires authorization. You can try <a class="docs-sign-in" href="#" data-bi-name="permission-content-sign-in">signing in</a> or <a  class="docs-change-directory" data-bi-name="permisson-content-change-directory">changing directories</a>.
					</p>
					<p class="margin-top-none authentication-determined authenticated">
						Access to this page requires authorization. You can try <a class="docs-change-directory" data-bi-name="permisson-content-change-directory">changing directories</a>.
					</p>
				</div>
			</div>
		</div>
	
					<div class="content"><h1 id="introduction-to-application-insights---opentelemetry-observability">Introduction to Application Insights - OpenTelemetry observability</h1></div>
					
		<div
			id="article-metadata"
			class="page-metadata-container display-flex gap-xxs justify-content-space-between align-items-center flex-wrap-wrap"
		>
			
		<div class="margin-block-xxs">
			<ul class="metadata page-metadata" data-bi-name="page info" lang="en-us" dir="ltr">
				  <li class="visibility-hidden-visual-diff">
			<local-time
				format="twoDigitNumeric"
				datetime="2025-08-26T22:16:00.000Z"
				data-article-date-source="calculated"
				class="is-invisible"
			>
				2025-08-26
			</local-time>
		</li>  
			</ul>
		</div>
	 
				<div
					id="user-feedback"
					class="margin-block-xxs display-none display-none-print"
					hidden
					data-hide-on-archived
				>
					
		<button
			id="user-feedback-button"
			data-test-id="conceptual-feedback-button"
			class="button button-sm button-clear button-primary display-none"
			type="button"
			data-bi-name="user-feedback-button"
			data-user-feedback-button
			hidden
		>
			<span class="icon" aria-hidden="true">
				<span class="docon docon-like"></span>
			</span>
			<span>Feedback</span>
		</button>
	
				</div>
		  
		</div>
	 
		<nav
			id="center-doc-outline"
			class="doc-outline is-hidden-desktop display-none-print margin-bottom-sm"
			data-bi-name="intopic toc"
			aria-label="In this article"
		>
			<h2 id="ms--in-this-article" class="title is-6 margin-block-xs">
				In this article
			</h2>
		</nav>
	
					<div class="content"><p>Azure Monitor Application Insights is an OpenTelemetry feature of <a href="../overview" data-linktype="relative-path">Azure Monitor</a> that offers application performance monitoring (APM) for live web applications. Integrating with OpenTelemetry (OTel) provides a vendor-neutral approach to collecting and analyzing telemetry data, enabling comprehensive observability of your applications.</p>
<p><span class="mx-imgBorder">
<a href="media/app-insights-overview/app-insights-overview-screenshot.png#lightbox" data-linktype="relative-path">
<img src="media/app-insights-overview/app-insights-overview-screenshot.png" alt="A screenshot of the Azure Monitor Application Insights user interface displaying an application map." data-linktype="relative-path">
</a>
</span>
</p>
<hr>
<h2 id="application-insights-experiences">Application Insights Experiences</h2>
<p>Application Insights supports OpenTelemetry (OTel) to collect telemetry data in a standardized format across platforms. Integration with Azure services allows for efficient monitoring and diagnostics, improving application observability and performance.</p>
<h3 id="investigate">Investigate</h3>
<ul>
<li><a href="overview-dashboard" data-linktype="relative-path">Application dashboard</a>: An at-a-glance assessment of your application's health and performance.</li>
<li><a href="app-map" data-linktype="relative-path">Application map</a>: A visual overview of application architecture and components' interactions.</li>
<li><a href="live-stream" data-linktype="relative-path">Live metrics</a>: A real-time analytics dashboard for insight into application activity and performance.</li>
<li><a href="transaction-search-and-diagnostics?tabs=transaction-search" data-linktype="relative-path">Transaction search</a>: Trace and diagnose transactions to identify issues and optimize performance.</li>
<li><a href="availability-overview" data-linktype="relative-path">Availability view</a>: Proactively monitor and test the availability and responsiveness of application endpoints.</li>
<li><a href="failures-and-performance-views?tabs=failures-view" data-linktype="relative-path">Failures view</a>: Identify and analyze failures in your application to minimize downtime.</li>
<li><a href="failures-and-performance-views?tabs=performance-view" data-linktype="relative-path">Performance view</a>: Review application performance metrics and potential bottlenecks.</li>
</ul>
<h3 id="monitoring">Monitoring</h3>
<ul>
<li><a href="../alerts/alerts-overview" data-linktype="relative-path">Alerts</a>: Monitor a wide range of aspects of your application and trigger various actions.</li>
<li><a href="../essentials/metrics-getting-started" data-linktype="relative-path">Metrics</a>: Dive deep into metrics data to understand usage patterns and trends.</li>
<li><a href="../essentials/diagnostic-settings" data-linktype="relative-path">Diagnostic settings</a>: Configure streaming export of platform logs and metrics to the destination of your choice.</li>
<li><a href="../logs/log-analytics-overview" data-linktype="relative-path">Logs</a>: Retrieve, consolidate, and analyze all data collected into Azure Monitoring Logs.</li>
<li><a href="../visualize/workbooks-overview" data-linktype="relative-path">Workbooks</a>: Create interactive reports and dashboards that visualize application monitoring data.</li>
</ul>
<h3 id="usage">Usage</h3>
<ul>
<li><a href="usage#users-sessions-and-events" data-linktype="relative-path">Users, sessions, and events</a>: Determine when, where, and how users interact with your web app.</li>
<li><a href="usage#funnels" data-linktype="relative-path">Funnels</a>: Analyze conversion rates to identify where users progress or drop off in the funnel.</li>
<li><a href="usage#user-flows" data-linktype="relative-path">Flows</a>: Visualize user paths on your site to identify high engagement areas and exit points.</li>
<li><a href="usage#cohorts" data-linktype="relative-path">Cohorts</a>: Group users by shared characteristics to simplify trend identification, segmentation, and performance troubleshooting.</li>
</ul>
<h3 id="code-analysis">Code analysis</h3>
<ul>
<li><a href="../profiler/profiler-overview" data-linktype="relative-path">.NET Profiler</a>: Capture, identify, and view performance traces for your application.</li>
<li><a href="../insights/code-optimizations" data-linktype="relative-path">Code optimizations</a>: Harness AI to create better and more efficient applications.</li>
<li><a href="../snapshot-debugger/snapshot-debugger" data-linktype="relative-path">Snapshot debugger</a>: Automatically collect debug snapshots when exceptions occur in .NET application</li>
</ul>
<hr>
<h2 id="logic-model">Logic model</h2>
<p>The logic model diagram visualizes components of Application Insights and how they interact.</p>
<p><span class="mx-imgBorder">
<a href="media/app-insights-overview/app-insights-overview-blowout.svg#lightbox" data-linktype="relative-path">
<img src="media/app-insights-overview/app-insights-overview-blowout.svg" alt="Diagram that shows the path of data as it flows through the layers of the Application Insights service." data-linktype="relative-path">
</a>
</span>
</p>
<div class="NOTE">
<p>Note</p>
<p>Firewall settings must be adjusted for data to reach ingestion endpoints. For more information, see <a href="../fundamentals/azure-monitor-network-access" data-linktype="relative-path">Azure Monitor endpoint access and firewall configuration</a>.</p>
</div>
<hr>
<h2 id="supported-languages">Supported languages</h2>
<p>This section outlines supported scenarios.</p>
<p>For more information about instrumenting applications to enable Application Insights, see <a href="opentelemetry-overview" data-linktype="relative-path">data collection basics</a>.</p>
<h3 id="manual-instrumentation">Manual instrumentation</h3>
<h4 id="opentelemetry-distro">OpenTelemetry Distro</h4>
<ul>
<li><a href="opentelemetry-enable?tabs=aspnetcore" data-linktype="relative-path">ASP.NET Core</a></li>
<li><a href="opentelemetry-enable?tabs=net" data-linktype="relative-path">.NET</a></li>
<li><a href="opentelemetry-enable?tabs=java" data-linktype="relative-path">Java</a></li>
<li><a href="opentelemetry-enable?tabs=nodejs" data-linktype="relative-path">Node.js</a></li>
<li><a href="opentelemetry-enable?tabs=python" data-linktype="relative-path">Python</a></li>
</ul>
<h4 id="client-side-javascript-sdk">Client-side JavaScript SDK</h4>
<ul>
<li><a href="javascript" data-linktype="relative-path">JavaScript</a>
<ul>
<li><a href="javascript-framework-extensions" data-linktype="relative-path">React</a></li>
<li><a href="javascript-framework-extensions" data-linktype="relative-path">React Native</a></li>
<li><a href="javascript-framework-extensions" data-linktype="relative-path">Angular</a></li>
</ul>
</li>
</ul>
<h4 id="application-insights-sdk-classic-api">Application Insights SDK (Classic API)</h4>
<div class="NOTE">
<p>Note</p>
<p>Review <a href="application-insights-faq#should-i-use-opentelemetry-or-the-application-insights-sdk" data-linktype="relative-path">Should I use OpenTelemetry or the Application Insights SDK</a> before considering instrumentation with the Classic API.</p>
</div>
<ul>
<li><a href="asp-net-core" data-linktype="relative-path">ASP.NET Core</a></li>
<li><a href="asp-net" data-linktype="relative-path">ASP.NET</a></li>
<li><a href="nodejs" data-linktype="relative-path">Node.js</a></li>
</ul>
<h3 id="automatic-instrumentation-enable-without-code-changes">Automatic instrumentation (enable without code changes)</h3>
<p>For supported environments and languages, see our <a href="codeless-overview#supported-environments-languages-and-resource-providers" data-linktype="relative-path">autoinstrumentation overview</a>.</p>
<h3 id="supported-platforms">Supported platforms</h3>
<h4 id="azure-service-integration-portal-enablement-azure-resource-manager-deployments">Azure service integration (portal enablement, Azure Resource Manager deployments)</h4>
<ul>
<li><a href="azure-vm-vmss-apps" data-linktype="relative-path">Azure Virtual Machines and Azure Virtual Machine Scale Sets</a></li>
<li><a href="azure-web-apps" data-linktype="relative-path">Azure App Service</a></li>
<li><a href="/en-us/azure/azure-functions/functions-monitoring" data-linktype="absolute-path">Azure Functions</a></li>
<li><a href="/en-us/azure/spring-apps/enterprise/how-to-application-insights" data-linktype="absolute-path">Azure Spring Apps</a></li>
<li><a href="azure-web-apps-net-core" data-linktype="relative-path">Azure Cloud Services</a>, including both web and worker roles</li>
</ul>
<h4 id="export-and-data-analysis">Export and data analysis</h4>
<ul>
<li><a href="../logs/log-powerbi" data-linktype="relative-path">Integrate Log Analytics with Power BI</a></li>
</ul>
<h3 id="unsupported-software-development-kits-sdks">Unsupported Software Development Kits (SDKs)</h3>
<p>Many community-supported Application Insights SDKs exist, but Microsoft only provides support for instrumentation options listed in this article.</p>
<hr>
<h2 id="troubleshooting">Troubleshooting</h2>
<p>For assistance with troubleshooting Application Insights, see <a href="/en-us/troubleshoot/azure/azure-monitor/welcome-azure-monitor" data-linktype="absolute-path">our dedicated troubleshooting documentation</a>.</p>
<hr>
<h2 id="help-and-support">Help and support</h2>
<h3 id="azure-technical-support">Azure technical support</h3>
<p>For Azure support issues, open an <a href="https://azure.microsoft.com/support/create-ticket/" data-linktype="external">Azure support ticket</a>.</p>
<h3 id="general-questions">General Questions</h3>
<p>Post general questions to the <a href="/en-us/answers/topics/24223/azure-monitor.html" data-linktype="absolute-path">Microsoft Questions and Answers forum</a>.</p>
<h3 id="coding-questions">Coding Questions</h3>
<p>Post coding questions to <a href="https://stackoverflow.com/questions/tagged/azure-application-insights" data-linktype="external">Stack Overflow</a> by using an <code>azure-application-insights</code> tag.</p>
<h3 id="feedback-community">Feedback Community</h3>
<p>Leave product feedback for the engineering team in the <a href="https://feedback.azure.com/d365community/forum/3887dc70-2025-ec11-b6e6-000d3a4f09d0" data-linktype="external">Feedback Community</a>.</p>
</div>
					
		<div
			id="ms--inline-notifications"
			class="margin-block-xs"
			data-bi-name="inline-notification"
		></div>
	 
		<div
			id="assertive-live-region"
			role="alert"
			aria-live="assertive"
			class="visually-hidden"
			aria-relevant="additions"
			aria-atomic="true"
		></div>
		<div
			id="polite-live-region"
			role="status"
			aria-live="polite"
			class="visually-hidden"
			aria-relevant="additions"
			aria-atomic="true"
		></div>
	
					
		<!-- feedback section -->
		<section
			id="site-user-feedback-footer"
			class="font-size-sm margin-top-md display-none-print display-none-desktop"
			data-test-id="site-user-feedback-footer"
			data-bi-name="site-feedback-section"
		>
			<hr class="hr" />
			<h2 id="ms--feedback" class="title is-3">Feedback</h2>
			<div class="display-flex flex-wrap-wrap align-items-center">
				<p class="font-weight-semibold margin-xxs margin-left-none">
					Was this page helpful?
				</p>
				<div class="buttons">
					<button
						class="thumb-rating-button like button button-primary button-sm"
						data-test-id="footer-rating-yes"
						data-binary-rating-response="rating-yes"
						type="button"
						title="This article is helpful"
						data-bi-name="button-rating-yes"
						aria-pressed="false"
					>
						<span class="icon" aria-hidden="true">
							<span class="docon docon-like"></span>
						</span>
						<span>Yes</span>
					</button>
					<button
						class="thumb-rating-button dislike button button-primary button-sm"
						data-test-id="footer-rating-no"
						data-binary-rating-response="rating-no"
						type="button"
						title="This article is not helpful"
						data-bi-name="button-rating-no"
						aria-pressed="false"
					>
						<span class="icon" aria-hidden="true">
							<span class="docon docon-dislike"></span>
						</span>
						<span>No</span>
					</button>
				</div>
			</div>
		</section>
		<!-- end feedback section -->
	
				</div>
				
		<div id="ms--additional-resources-mobile" class="display-none-print">
			<hr class="hr" hidden />
			<h2 id="ms--additional-resources-mobile-heading" class="title is-3" hidden>
				Additional resources
			</h2>
			
		<section
			id="right-rail-recommendations-mobile"
			class=""
			data-bi-name="recommendations"
			hidden
		></section>
	 
		<section
			id="right-rail-training-mobile"
			class=""
			data-bi-name="learning-resource-card"
			hidden
		></section>
	 
		<section
			id="right-rail-events-mobile"
			class=""
			data-bi-name="events-card"
			hidden
		></section>
	 
		<section
			id="right-rail-qna-mobile"
			class="margin-top-xxs"
			data-bi-name="qna-link-card"
			hidden
		></section>
	
		</div>
	
			</div>
			
		<div
			id="action-panel"
			role="region"
			aria-label="Action Panel"
			class="action-panel"
			tabindex="-1"
		></div>
	
		
				</main>
				<aside
					id="layout-body-aside"
					class="layout-body-aside "
					data-bi-name="aside"
			  >
					
		<div
			id="ms--additional-resources"
			class="right-container padding-sm display-none display-block-desktop height-full"
			data-bi-name="pageactions"
			role="complementary"
			aria-label="Additional resources"
		>
			<div id="affixed-right-container" data-bi-name="right-column">
				
		<nav
			id="side-doc-outline"
			class="doc-outline border-bottom padding-bottom-xs margin-bottom-xs"
			data-bi-name="intopic toc"
			aria-label="In this article"
		>
			<h3>In this article</h3>
		</nav>
	
				<!-- Feedback -->
				
		<section
			id="ms--site-user-feedback-right-rail"
			class="font-size-sm display-none-print"
			data-test-id="site-user-feedback-right-rail"
			data-bi-name="site-feedback-right-rail"
		>
			<p class="font-weight-semibold margin-bottom-xs">Was this page helpful?</p>
			<div class="buttons">
				<button
					class="thumb-rating-button like button button-primary button-sm"
					data-test-id="right-rail-rating-yes"
					data-binary-rating-response="rating-yes"
					type="button"
					title="This article is helpful"
					data-bi-name="button-rating-yes"
					aria-pressed="false"
				>
					<span class="icon" aria-hidden="true">
						<span class="docon docon-like"></span>
					</span>
					<span>Yes</span>
				</button>
				<button
					class="thumb-rating-button dislike button button-primary button-sm"
					data-test-id="right-rail-rating-no"
					data-binary-rating-response="rating-no"
					type="button"
					title="This article is not helpful"
					data-bi-name="button-rating-no"
					aria-pressed="false"
				>
					<span class="icon" aria-hidden="true">
						<span class="docon docon-dislike"></span>
					</span>
					<span>No</span>
				</button>
			</div>
		</section>
	
			</div>
		</div>
	
			  </aside> <section
					id="layout-body-flyout"
					class="layout-body-flyout "
					data-bi-name="flyout"
			  >
					 <div
	class="height-full border-left background-color-body-medium"
	id="ask-learn-flyout"
></div>
			  </section> <div class="layout-body-footer " data-bi-name="layout-footer">
		<footer
			id="footer"
			data-test-id="footer"
			data-bi-name="footer"
			class="footer-layout has-padding has-default-focus border-top  uhf-container"
			role="contentinfo"
		>
			<div class="display-flex gap-xs flex-wrap-wrap is-full-height padding-right-lg-desktop">
				
		<a
			data-mscc-ic="false"
			href="#"
			data-bi-name="select-locale"
			class="locale-selector-link flex-shrink-0 button button-sm button-clear external-link-indicator"
			id=""
			title=""
			><span class="icon" aria-hidden="true"
				><span class="docon docon-world"></span></span
			><span class="local-selector-link-text">en-us</span></a
		>
	
				<div class="ccpa-privacy-link" data-ccpa-privacy-link hidden>
		
		<a
			data-mscc-ic="false"
			href="https://aka.ms/yourcaliforniaprivacychoices"
			data-bi-name="your-privacy-choices"
			class="button button-sm button-clear flex-shrink-0 external-link-indicator"
			id=""
			title=""
			>
		<svg
			xmlns="http://www.w3.org/2000/svg"
			viewBox="0 0 30 14"
			xml:space="preserve"
			height="16"
			width="43"
			aria-hidden="true"
			focusable="false"
		>
			<path
				d="M7.4 12.8h6.8l3.1-11.6H7.4C4.2 1.2 1.6 3.8 1.6 7s2.6 5.8 5.8 5.8z"
				style="fill-rule:evenodd;clip-rule:evenodd;fill:#fff"
			></path>
			<path
				d="M22.6 0H7.4c-3.9 0-7 3.1-7 7s3.1 7 7 7h15.2c3.9 0 7-3.1 7-7s-3.2-7-7-7zm-21 7c0-3.2 2.6-5.8 5.8-5.8h9.9l-3.1 11.6H7.4c-3.2 0-5.8-2.6-5.8-5.8z"
				style="fill-rule:evenodd;clip-rule:evenodd;fill:#06f"
			></path>
			<path
				d="M24.6 4c.2.2.2.6 0 .8L22.5 7l2.2 2.2c.2.2.2.6 0 .8-.2.2-.6.2-.8 0l-2.2-2.2-2.2 2.2c-.2.2-.6.2-.8 0-.2-.2-.2-.6 0-.8L20.8 7l-2.2-2.2c-.2-.2-.2-.6 0-.8.2-.2.6-.2.8 0l2.2 2.2L23.8 4c.2-.2.6-.2.8 0z"
				style="fill:#fff"
			></path>
			<path
				d="M12.7 4.1c.2.2.3.6.1.8L8.6 9.8c-.1.1-.2.2-.3.2-.2.1-.5.1-.7-.1L5.4 7.7c-.2-.2-.2-.6 0-.8.2-.2.6-.2.8 0L8 8.6l3.8-4.5c.2-.2.6-.2.9 0z"
				style="fill:#06f"
			></path>
		</svg>
	
			<span>Your Privacy Choices</span></a
		>
	
	</div>
				<div class="flex-shrink-0">
		<div class="dropdown has-caret-up">
			<button
				data-test-id="theme-selector-button"
				class="dropdown-trigger button button-clear button-sm has-inner-focus theme-dropdown-trigger"
				aria-controls="{{ themeMenuId }}"
				aria-expanded="false"
				title="Theme"
				data-bi-name="theme"
			>
				<span class="icon">
					<span class="docon docon-sun" aria-hidden="true"></span>
				</span>
				<span>Theme</span>
				<span class="icon expanded-indicator" aria-hidden="true">
					<span class="docon docon-chevron-down-light"></span>
				</span>
			</button>
			<div class="dropdown-menu" id="{{ themeMenuId }}" role="menu">
				<ul class="theme-selector padding-xxs" data-test-id="theme-dropdown-menu">
					<li class="theme display-block">
						<button
							class="button button-clear button-sm theme-control button-block justify-content-flex-start text-align-left"
							data-theme-to="light"
						>
							<span class="theme-light margin-right-xxs">
								<span
									class="theme-selector-icon border display-inline-block has-body-background"
									aria-hidden="true"
								>
									<svg class="svg" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 22 14">
										<rect width="22" height="14" class="has-fill-body-background" />
										<rect x="5" y="5" width="12" height="4" class="has-fill-secondary" />
										<rect x="5" y="2" width="2" height="1" class="has-fill-secondary" />
										<rect x="8" y="2" width="2" height="1" class="has-fill-secondary" />
										<rect x="11" y="2" width="3" height="1" class="has-fill-secondary" />
										<rect x="1" y="1" width="2" height="2" class="has-fill-secondary" />
										<rect x="5" y="10" width="7" height="2" rx="0.3" class="has-fill-primary" />
										<rect x="19" y="1" width="2" height="2" rx="1" class="has-fill-secondary" />
									</svg>
								</span>
							</span>
							<span role="menuitem"> Light </span>
						</button>
					</li>
					<li class="theme display-block">
						<button
							class="button button-clear button-sm theme-control button-block justify-content-flex-start text-align-left"
							data-theme-to="dark"
						>
							<span class="theme-dark margin-right-xxs">
								<span
									class="border theme-selector-icon display-inline-block has-body-background"
									aria-hidden="true"
								>
									<svg class="svg" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 22 14">
										<rect width="22" height="14" class="has-fill-body-background" />
										<rect x="5" y="5" width="12" height="4" class="has-fill-secondary" />
										<rect x="5" y="2" width="2" height="1" class="has-fill-secondary" />
										<rect x="8" y="2" width="2" height="1" class="has-fill-secondary" />
										<rect x="11" y="2" width="3" height="1" class="has-fill-secondary" />
										<rect x="1" y="1" width="2" height="2" class="has-fill-secondary" />
										<rect x="5" y="10" width="7" height="2" rx="0.3" class="has-fill-primary" />
										<rect x="19" y="1" width="2" height="2" rx="1" class="has-fill-secondary" />
									</svg>
								</span>
							</span>
							<span role="menuitem"> Dark </span>
						</button>
					</li>
					<li class="theme display-block">
						<button
							class="button button-clear button-sm theme-control button-block justify-content-flex-start text-align-left"
							data-theme-to="high-contrast"
						>
							<span class="theme-high-contrast margin-right-xxs">
								<span
									class="border theme-selector-icon display-inline-block has-body-background"
									aria-hidden="true"
								>
									<svg class="svg" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 22 14">
										<rect width="22" height="14" class="has-fill-body-background" />
										<rect x="5" y="5" width="12" height="4" class="has-fill-secondary" />
										<rect x="5" y="2" width="2" height="1" class="has-fill-secondary" />
										<rect x="8" y="2" width="2" height="1" class="has-fill-secondary" />
										<rect x="11" y="2" width="3" height="1" class="has-fill-secondary" />
										<rect x="1" y="1" width="2" height="2" class="has-fill-secondary" />
										<rect x="5" y="10" width="7" height="2" rx="0.3" class="has-fill-primary" />
										<rect x="19" y="1" width="2" height="2" rx="1" class="has-fill-secondary" />
									</svg>
								</span>
							</span>
							<span role="menuitem"> High contrast </span>
						</button>
					</li>
				</ul>
			</div>
		</div>
	</div>
			</div>
			<ul class="links" data-bi-name="footerlinks">
				<li class="manage-cookies-holder" hidden=""></li>
				<li>
		
		<a
			data-mscc-ic="false"
			href="https://learn.microsoft.com/en-us/principles-for-ai-generated-content"
			data-bi-name="aiDisclaimer"
			class=" external-link-indicator"
			id=""
			title=""
			>AI Disclaimer</a
		>
	
	</li><li>
		
		<a
			data-mscc-ic="false"
			href="https://learn.microsoft.com/en-us/previous-versions/"
			data-bi-name="archivelink"
			class=" external-link-indicator"
			id=""
			title=""
			>Previous Versions</a
		>
	
	</li> <li>
		
		<a
			data-mscc-ic="false"
			href="https://techcommunity.microsoft.com/t5/microsoft-learn-blog/bg-p/MicrosoftLearnBlog"
			data-bi-name="bloglink"
			class=" external-link-indicator"
			id=""
			title=""
			>Blog</a
		>
	
	</li> <li>
		
		<a
			data-mscc-ic="false"
			href="https://learn.microsoft.com/en-us/contribute"
			data-bi-name="contributorGuide"
			class=" external-link-indicator"
			id=""
			title=""
			>Contribute</a
		>
	
	</li><li>
		
		<a
			data-mscc-ic="false"
			href="https://go.microsoft.com/fwlink/?LinkId=521839"
			data-bi-name="privacy"
			class=" external-link-indicator"
			id=""
			title=""
			>Privacy</a
		>
	
	</li><li>
		
		<a
			data-mscc-ic="false"
			href="https://learn.microsoft.com/en-us/legal/termsofuse"
			data-bi-name="termsofuse"
			class=" external-link-indicator"
			id=""
			title=""
			>Terms of Use</a
		>
	
	</li><li>
		
		<a
			data-mscc-ic="false"
			href="https://www.microsoft.com/legal/intellectualproperty/Trademarks/"
			data-bi-name="trademarks"
			class=" external-link-indicator"
			id=""
			title=""
			>Trademarks</a
		>
	
	</li>
				<li>&copy; Microsoft 2025</li>
			</ul>
		</footer>
	</footer>
			</body>
		</html>

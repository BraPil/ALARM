Downloaded from: https://learn.microsoft.com/en-us/dotnet/core/porting/

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
			<title>Port from .NET Framework to .NET - .NET Core | Microsoft Learn</title>
			<meta charset="utf-8" />
			<meta name="viewport" content="width=device-width, initial-scale=1.0" />
			<meta name="color-scheme" content="light dark" />

			<meta name="description" content="Understand the porting process and discover tools you might find helpful when porting a .NET Framework project to .NET." />
			<link rel="canonical" href="https://learn.microsoft.com/en-us/dotnet/core/porting/" /> 

			<!-- Non-customizable open graph and sharing-related metadata -->
			<meta name="twitter:card" content="summary" />
			<meta name="twitter:site" content="@MicrosoftLearn" />
			<meta property="og:type" content="website" />
			<meta property="og:image:alt" content="Port from .NET Framework to .NET - .NET Core | Microsoft Learn" />
			<meta property="og:image" content="https://learn.microsoft.com/dotnet/media/dotnet-logo.png" />
			<!-- Page specific open graph and sharing-related metadata -->
			<meta property="og:title" content="Port from .NET Framework to .NET - .NET Core" />
			<meta property="og:url" content="https://learn.microsoft.com/en-us/dotnet/core/porting/" />
			<meta property="og:description" content="Understand the porting process and discover tools you might find helpful when porting a .NET Framework project to .NET." />
			<meta name="platform_id" content="c7231d7d-ce65-b9ad-4abf-2e3f210a8ba6" /> <meta name="scope" content=".NET" />
			<meta name="locale" content="en-us" />
			 <meta name="adobe-target" content="true" />
			<meta name="uhfHeaderId" content="MSDocsHeader-DotNet" />

			<meta name="page_type" content="conceptual" />

			<!--page specific meta tags-->
			

			<!-- custom meta tags -->
			
		<meta name="apiPlatform" content="dotnet" />
	
		<meta name="author" content="adegeo" />
	
		<meta name="breadcrumb_path" content="/dotnet/breadcrumb/toc.json" />
	
		<meta name="feedback_system" content="OpenSource" />
	
		<meta name="feedback_product_url" content="https://aka.ms/feedback/report?space=61" />
	
		<meta name="ms.author" content="dotnetcontent" />
	
		<meta name="ms.devlang" content="dotnet" />
	
		<meta name="ms.service" content="dotnet-fundamentals" />
	
		<meta name="ms.topic" content="article" />
	
		<meta name="show_latex" content="true" />
	
		<meta name="ms.date" content="2025-06-03T00:00:00Z" />
	
		<meta name="ms.custom" content="devdivchpfy22, updateeachrelease" />
	
		<meta name="document_id" content="577db847-e424-a9dc-ee2d-f2c6ddb54600" />
	
		<meta name="document_version_independent_id" content="5fa219d8-0c9c-c758-c1ee-17cefd75ca24" />
	
		<meta name="updated_at" content="2025-07-21T14:09:00Z" />
	
		<meta name="original_content_git_url" content="https://github.com/dotnet/docs/blob/live/docs/core/porting/index.md" />
	
		<meta name="gitcommit" content="https://github.com/dotnet/docs/blob/7d53b3e2224984212208e869d4602cb8f1a6d016/docs/core/porting/index.md" />
	
		<meta name="git_commit_id" content="7d53b3e2224984212208e869d4602cb8f1a6d016" />
	
		<meta name="site_name" content="Docs" />
	
		<meta name="depot_name" content="VS.core-docs" />
	
		<meta name="schema" content="Conceptual" />
	
		<meta name="toc_rel" content="../../navigate/migration-guide/toc.json" />
	
		<meta name="pdf_url_template" content="https://learn.microsoft.com/pdfstore/en-us/VS.core-docs/{branchName}{pdfName}" />
	
		<meta name="feedback_help_link_type" content="" />
	
		<meta name="feedback_help_link_url" content="" />
	
		<meta name="search.mshattr.devlang" content="csharp" />
	
		<meta name="word_count" content="2222" />
	
		<meta name="asset_id" content="core/porting/index" />
	
		<meta name="moniker_range_name" content="" />
	
		<meta name="item_type" content="Content" />
	
		<meta name="source_path" content="docs/core/porting/index.md" />
	
		<meta name="previous_tlsh_hash" content="724736529509A73D6EC3D87A6097AA1095F09405CEB069CD112561D1976A3F76ABEA27B7E74F9318467043B20586668A1BC2D76E71BC7F532758283CC30C32A1D2953B73BB" />
	
		<meta name="github_feedback_content_git_url" content="https://github.com/dotnet/docs/blob/main/docs/core/porting/index.md" />
	 
		<meta name="cmProducts" content="https://authoring-docs-microsoft.poolparty.biz/devrel/7696cda6-0510-47f6-8302-71bb5d2e28cf" data-source="generated" />
	
		<meta name="spProducts" content="https://authoring-docs-microsoft.poolparty.biz/devrel/69c76c32-967e-4c65-b89a-74cc527db725" data-source="generated" />
	

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
    "brand": "",
    "context": {},
    "standardFeedback": false,
    "showFeedbackReport": false,
    "feedbackHelpLinkType": "",
    "feedbackHelpLinkUrl": "",
    "feedbackSystem": "OpenSource",
    "feedbackGitHubRepo": "dotnet/docs",
    "feedbackProductUrl": "https://aka.ms/feedback/report?space=61",
    "extendBreadcrumb": false,
    "isEditDisplayable": true,
    "isPrivateUnauthorized": false,
    "hideViewSource": false,
    "isPermissioned": false,
    "hasRecommendations": true,
    "contributors": [
      {
        "name": "adegeo",
        "url": "https://github.com/adegeo"
      },
      {
        "name": "Caoxuyang",
        "url": "https://github.com/Caoxuyang"
      },
      {
        "name": "gewarren",
        "url": "https://github.com/gewarren"
      },
      {
        "name": "conniey",
        "url": "https://github.com/conniey"
      },
      {
        "name": "Nick-Stanton",
        "url": "https://github.com/Nick-Stanton"
      },
      {
        "name": "danmoseley",
        "url": "https://github.com/danmoseley"
      },
      {
        "name": "reflectronic",
        "url": "https://github.com/reflectronic"
      },
      {
        "name": "lukehammer",
        "url": "https://github.com/lukehammer"
      },
      {
        "name": "Rick-Anderson",
        "url": "https://github.com/Rick-Anderson"
      },
      {
        "name": "poojapoojari",
        "url": "https://github.com/poojapoojari"
      },
      {
        "name": "neman",
        "url": "https://github.com/neman"
      },
      {
        "name": "mjrousos",
        "url": "https://github.com/mjrousos"
      },
      {
        "name": "brian218",
        "url": "https://github.com/brian218"
      },
      {
        "name": "IEvangelist",
        "url": "https://github.com/IEvangelist"
      },
      {
        "name": "cartermp",
        "url": "https://github.com/cartermp"
      },
      {
        "name": "Maples7",
        "url": "https://github.com/Maples7"
      },
      {
        "name": "twsouthwick",
        "url": "https://github.com/twsouthwick"
      },
      {
        "name": "mairaw",
        "url": "https://github.com/mairaw"
      },
      {
        "name": "poychang",
        "url": "https://github.com/poychang"
      },
      {
        "name": "Youssef1313",
        "url": "https://github.com/Youssef1313"
      },
      {
        "name": "Lxiamail",
        "url": "https://github.com/Lxiamail"
      },
      {
        "name": "nschonni",
        "url": "https://github.com/nschonni"
      },
      {
        "name": "DennisLee-DennisLee",
        "url": "https://github.com/DennisLee-DennisLee"
      },
      {
        "name": "terrajobst",
        "url": "https://github.com/terrajobst"
      },
      {
        "name": "Mikejo5000",
        "url": "https://github.com/Mikejo5000"
      },
      {
        "name": "tompratt-AQ",
        "url": "https://github.com/tompratt-AQ"
      },
      {
        "name": "guardrex",
        "url": "https://github.com/guardrex"
      },
      {
        "name": "zwcloud",
        "url": "https://github.com/zwcloud"
      },
      {
        "name": "serialseb",
        "url": "https://github.com/serialseb"
      },
      {
        "name": "tdykstra",
        "url": "https://github.com/tdykstra"
      }
    ],
    "mathjax": {},
    "openSourceFeedbackIssueUrl": "https://github.com/dotnet/docs/issues/new?template=z-customer-feedback.yml",
    "openSourceFeedbackIssueTitle": ""
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
			
			href="https://github.com/dotnet/docs/blob/main/docs/core/porting/index.md"
			data-original_content_git_url="https://github.com/dotnet/docs/blob/live/docs/core/porting/index.md"
			data-original_content_git_url_template="{repo}/blob/{branch}/docs/core/porting/index.md"
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
	
					<div class="content"><h1 id="overview-of-porting-from-net-framework-to-net">Overview of porting from .NET Framework to .NET</h1></div>
					
		<div
			id="article-metadata"
			class="page-metadata-container display-flex gap-xxs justify-content-space-between align-items-center flex-wrap-wrap"
		>
			
		<div class="margin-block-xxs">
			<ul class="metadata page-metadata" data-bi-name="page info" lang="en-us" dir="ltr">
				  <li class="visibility-hidden-visual-diff">
			<local-time
				format="twoDigitNumeric"
				datetime="2025-07-21T14:09:00.000Z"
				data-article-date-source="calculated"
				class="is-invisible"
			>
				2025-07-21
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
	
					<div class="content"><p>This article provides an overview of what you should consider when porting your code from .NET Framework to .NET (formerly named .NET Core). Porting to .NET from .NET Framework is relatively straightforward for many projects. The complexity of your projects dictates how much work you'll need to do after the initial migration of the project files.</p>
<p>Projects where the app model is available in .NET, such as libraries, console apps, and desktop apps, usually require little change. Projects that require a new app model, such as moving to <a href="/en-us/aspnet/core/migration/proper-to-2x/" data-linktype="absolute-path">ASP.NET Core from ASP.NET</a>, require more work. Many patterns from the old app model have equivalents that can be used during the conversion.</p>
<h2 id="windows-desktop-technologies">Windows desktop technologies</h2>
<p>Many applications created for .NET Framework use a desktop technology such as Windows Forms or Windows Presentation Foundation (WPF). Both Windows Forms and WPF are available in .NET, but they remain Windows-only technologies.</p>
<p>Consider the following dependencies before you migrate a Windows Forms or WPF application:</p>
<ul>
<li>Project files for .NET use a different format than .NET Framework.</li>
<li>Your project might use an API that isn't available in .NET.</li>
<li>Third-party controls and libraries might not have been ported to .NET and remain only available to .NET Framework.</li>
<li>Your project uses a <a href="net-framework-tech-unavailable" data-linktype="relative-path">technology that is no longer available</a> in .NET.</li>
</ul>
<p>.NET uses the open-source versions of Windows Forms and WPF and includes enhancements over .NET Framework.</p>
<p>For tutorials on migrating your desktop application to .NET, see one of the following articles:</p>
<ul>
<li><a href="/en-us/dotnet/desktop/wpf/migration/" data-linktype="absolute-path">How to upgrade a WPF desktop app to .NET</a></li>
<li><a href="/en-us/dotnet/desktop/winforms/migration/" data-linktype="absolute-path">Migrate .NET Framework Windows Forms apps to .NET</a></li>
</ul>
<h2 id="windows-specific-apis">Windows-specific APIs</h2>
<p>Applications can still P/Invoke native libraries on platforms supported by .NET. This technology isn't limited to Windows. However, if the library you're referencing is Windows-specific, such as a <em>user32.dll</em> or <em>kernel32.dll</em>, then the code only works on Windows. For each platform you want your app to run on, you have to either find platform-specific versions, or make your code generic enough to run on all platforms.</p>
<p>When you're porting an application from .NET Framework to .NET, your application probably used a library provided by .NET Framework. Many APIs that were available in .NET Framework weren't ported to .NET because they relied on Windows-specific technology, such as the Windows Registry or the GDI+ drawing model.</p>
<p>The <strong>Windows Compatibility Pack</strong> provides a large portion of the .NET Framework API surface to .NET and is provided via the <a href="https://www.nuget.org/packages/Microsoft.Windows.Compatibility" data-linktype="external">Microsoft.Windows.Compatibility NuGet package</a>.</p>
<p>For more information, see <a href="windows-compat-pack" data-linktype="relative-path">Use the Windows Compatibility Pack to port code to .NET</a>.</p>
<h2 id="net-framework-compatibility-mode">.NET Framework compatibility mode</h2>
<p>The .NET Framework compatibility mode was introduced in .NET Standard 2.0. The compatibility mode allows .NET Standard and .NET projects to reference .NET Framework libraries as if they were compiled for the project's target framework. However, some .NET implementations might support a larger chunk of .NET Framework than others. For example, .NET Core 3.0 extends the .NET Framework compatibility mode to Windows Forms and WPF. Referencing .NET Framework libraries doesn't work for all projects, such as if the library uses WPF APIs, but it does unblock many porting scenarios. For more information, see the <a href="third-party-deps#net-framework-compatibility-mode" data-linktype="relative-path">Analyze your dependencies to port code from .NET Framework to .NET</a>.</p>
<p>Referencing .NET Framework libraries doesn't work in all cases, as it depends on which .NET Framework APIs were used and whether or not these APIs are supported by the project's target framework. Also, some of the .NET Framework APIs will only work on Windows. The .NET Framework compatibility mode unblocks many porting scenarios but you should test your projects to ensure that they also work at runtime. For more information, see the <a href="third-party-deps#net-framework-compatibility-mode" data-linktype="relative-path">Analyze your dependencies to port code from .NET Framework to</a>.</p>
<h2 id="target-framework-changes-in-sdk-style-projects">Target framework changes in SDK-style projects</h2>
<p>As previously mentioned, the project files for .NET use a different format than .NET Framework, known as the SDK-style project format. Even if you're not moving from .NET Framework to .NET, you should still upgrade the project file to the latest format. The way to specify a target framework is different in SDK-style projects. In .NET Framework, the <code>&lt;TargetFrameworkVersion&gt;</code> property is used with a moniker that specifies the version of .NET Framework. For example, .NET Framework 4.7.2 looks like the following snippet:</p>
<pre><code class="lang-xml">&lt;PropertyGroup&gt;
  &lt;TargetFrameworkVersion&gt;v4.7.2&lt;/TargetFrameworkVersion&gt;
&lt;/PropertyGroup&gt;
</code></pre>
<p>An SDK-style project uses a different property to identify the target framework, the <code>&lt;TargetFramework&gt;</code> property. When targeting .NET Framework, the moniker starts with <code>net</code> and ends with the version of .NET Framework without any periods. For example, the moniker to target .NET Framework 4.7.2 is <code>net472</code>:</p>
<pre><code class="lang-xml">&lt;PropertyGroup&gt;
  &lt;TargetFramework&gt;net472&lt;/TargetFramework&gt;
&lt;/PropertyGroup&gt;
</code></pre>
<p>For a list of all target monikers, see <a href="../../standard/frameworks#supported-target-frameworks" data-linktype="relative-path">Target frameworks in SDK-style projects</a>.</p>
<h2 id="unavailable-technologies">Unavailable technologies</h2>
<p>There are a few technologies in .NET Framework that don't exist in .NET:</p>
<ul>
<li><p><a href="net-framework-tech-unavailable#application-domains" data-linktype="relative-path">Application domains</a></p>
<p>Creating additional application domains isn't supported. For code isolation, use separate processes or containers as an alternative.</p>
</li>
<li><p><a href="net-framework-tech-unavailable#remoting" data-linktype="relative-path">Remoting</a></p>
<p>Remoting is used for communicating across application domains, which are no longer supported. For simple communication across processes, consider inter-process communication (IPC) mechanisms as an alternative to remoting, such as the <a href="/en-us/dotnet/api/system.io.pipes" class="no-loc" data-linktype="absolute-path">System.IO.Pipes</a> class or the <a href="/en-us/dotnet/api/system.io.memorymappedfiles.memorymappedfile" class="no-loc" data-linktype="absolute-path">MemoryMappedFile</a> class. For more complex scenarios, consider frameworks such as <a href="https://github.com/microsoft/vs-streamjsonrpc" data-linktype="external">StreamJsonRpc</a> or <a href="/en-us/aspnet/core" data-linktype="absolute-path">ASP.NET Core</a> (either using <a href="/en-us/aspnet/core/grpc" data-linktype="absolute-path">gRPC</a> or <a href="/en-us/aspnet/core/web-api" data-linktype="absolute-path">RESTful Web API services</a>).</p>
<p>Because remoting isn't supported, calls to <code>BeginInvoke()</code> and <code>EndInvoke()</code> on delegate objects will throw <code>PlatformNotSupportedException</code>.</p>
</li>
<li><p><a href="net-framework-tech-unavailable#code-access-security-cas" data-linktype="relative-path">Code access security (CAS)</a></p>
<p>CAS was a sandboxing technique supported by .NET Framework but deprecated in .NET Framework 4.0. It was replaced by Security Transparency and it isn't supported in .NET. Instead, use security boundaries provided by the operating system, such as virtualization, containers, or user accounts.</p>
</li>
<li><p><a href="net-framework-tech-unavailable#security-transparency" data-linktype="relative-path">Security transparency</a></p>
<p>Similar to CAS, the security transparency sandboxing technique is no longer recommended for .NET Framework applications and it isn't supported in .NET. Instead, use security boundaries provided by the operating system, such as virtualization, containers, or user accounts.</p>
</li>
<li><p><a href="/en-us/dotnet/api/system.enterpriseservices" class="no-loc" data-linktype="absolute-path">System.EnterpriseServices</a></p>
<p><a href="/en-us/dotnet/api/system.enterpriseservices" class="no-loc" data-linktype="absolute-path">System.EnterpriseServices</a> (COM+) isn't supported in .NET.</p>
</li>
<li><p>Windows Workflow Foundation (WF)</p>
<p>WF isn't supported in .NET. For an alternative, see <a href="https://github.com/UiPath/corewf" data-linktype="external">CoreWF</a>.</p>
</li>
</ul>
<p>For more information about these unsupported technologies, see <a href="net-framework-tech-unavailable" data-linktype="relative-path">.NET Framework technologies unavailable on .NET 6+</a>.</p>
<h2 id="cross-platform">Cross-platform</h2>
<p>.NET (formerly known as .NET Core) is designed to be cross-platform. If your code doesn't depend on Windows-specific technologies, it can run on other platforms such as macOS, Linux, and Android. Such code includes project types like:</p>
<ul>
<li>Libraries</li>
<li>Console-based tools</li>
<li>Automation</li>
<li>ASP.NET sites</li>
</ul>
<p>.NET Framework is a Windows-only component. When your code uses Windows-specific technologies or APIs, such as Windows Forms and WPF, the code can still run on .NET but it doesn't run on other operating systems.</p>
<p>It's possible that your library or console-based application can be used cross-platform without changing much. When you're porting to .NET, you might want to take this into consideration and test your application on other platforms.</p>
<h2 id="the-future-of-net-standard">The future of .NET Standard</h2>
<p>.NET Standard is a formal specification of .NET APIs that are available on multiple .NET implementations. The motivation behind .NET Standard was to establish greater uniformity in the .NET ecosystem. Starting with .NET 5, a different approach to establishing uniformity has been adopted, and this new approach eliminates the need for .NET Standard in many scenarios. For more information, see <a href="../../standard/net-standard#net-5-and-net-standard" data-linktype="relative-path">.NET 5+ and .NET Standard</a>.</p>
<p>.NET Standard 2.0 was the last version to support .NET Framework.</p>
<h2 id="tools-to-assist-porting">Tools to assist porting</h2>
<p>Instead of manually porting an application from .NET Framework to .NET, you can use different tools to help automate some aspects of the migration. Porting a complex project is, in itself, a complex process. The tools might help in that journey.</p>
<p>Even if you use a tool to help port your application, you should review the <a href="#considerations-when-porting" data-linktype="self-bookmark">Considerations when porting section</a> in this article.</p>
<h3 id="github-copilot-app-modernization--upgrade-for-net">GitHub Copilot App Modernization â Upgrade for .NET</h3>
<p><a href="github-copilot-app-modernization-overview" data-linktype="relative-path">GitHub Copilot App Modernization â Upgrade for .NET</a> is a Visual Studio extension that helps you upgrade projects to newer versions of .NET, update dependencies, and apply code fixes. It leverages GitHub Copilot to provide an interactive upgrade experience.</p>
<p>This tool supports the following upgrade paths:</p>
<ul>
<li>Upgrade projects from .NET Core to .NET.</li>
<li>Upgrade projects from older versions of .NET to the latest.</li>
<li>Modernize your code base.</li>
</ul>
<p><strong>When to use:</strong></p>
<p>Use GitHub Copilot App Modernization â Upgrade for .NET for scenarios where you want to upgrade your .NET project code and dependencies to newer versions of .NET using an AI-powered tool.</p>
<h3 id="github-copilot-app-modernization-for-net">GitHub Copilot app modernization for .NET</h3>
<p>GitHub Copilot app modernization for .NET (Preview) helps you migrate .NET applications to Azure efficiently and confidently. Powered by GitHub Copilot and <a href="../../azure/migration/appcat/app-code-assessment-toolkit" data-linktype="relative-path">Application and code assessment for .NET</a>, it guides you through assessment, solution recommendations, code fixes, and validationâall within a single tool.</p>
<p>With this assistant, you can:</p>
<ul>
<li>Assess your application's code, configuration, and dependencies.</li>
<li>Plan and set up the right Azure resources.</li>
<li>Fix issues and apply best practices for cloud migration.</li>
<li>Validate that your app builds and tests successfully.</li>
</ul>
<p>For more details, see the <a href="upgrade-assistant-overview" data-linktype="relative-path">GitHub Copilot app modernization for .NET overview</a>.</p>
<p><strong>When to use:</strong></p>
<p>Use the GitHub Copilot app modernization for .NET (Preview) experience for scenarios where you need end to end assessment, planning, and remediation for migrating your .NET apps to Azure.</p>
<h3 id="application-and-code-assessment-for-net">Application and Code Assessment for .NET</h3>
<p><a href="../../azure/migration/appcat/app-code-assessment-toolkit" data-linktype="relative-path">Azure Migrate application and code assessment for .NET</a> provides code and application analysis, along with recommendations for planning cloud deployments. It helps you confidently run business-critical solutions in the cloud by offering a developer-focused assessment of your source code. The tool also provides recommendations and examples to optimize code and configurations for Azure, following industry best practices.</p>
<p>This tool is also used by the GitHub Copilot app modernization for .NET experience.</p>
<p><strong>When to use:</strong></p>
<p>Use the Azure Migrate application and code assessment for .NET toolset for an assessment of and recommendations for migrating an existing code base to Azure. The Azure Migrate application and code assessment is essentially a subset of the GitHub Copilot app modernization for .NET experience.</p>
<h3 id="net-upgrade-assistant">.NET Upgrade Assistant</h3>
<p>The <a href="upgrade-assistant-overview" data-linktype="relative-path">.NET Upgrade Assistant</a> is a command-line tool that can be run on different kinds of .NET Framework apps. It's designed to assist with upgrading .NET Framework apps to .NET. After running the tool, <strong>in most cases the app will require more effort to complete the migration</strong>. The tool includes the installation of analyzers that can assist with completing the migration. This tool works on the following types of .NET Framework applications:</p>
<ul>
<li>Windows Forms</li>
<li>WPF</li>
<li>ASP.NET MVC</li>
<li>Console</li>
<li>Class libraries</li>
</ul>
<p>This tool uses the other tools listed in this article, such as <strong>try-convert</strong>, and guides the migration process. For more information about the tool, see <a href="upgrade-assistant-overview" data-linktype="relative-path">Overview of the .NET Upgrade Assistant</a>.</p>
<p><strong>When to use:</strong></p>
<p>Use .NET Upgrade Assistant to upgrade .NET Framework apps to newer versions of .NET. This tool provides an alternative to the AI powered GitHub Copilot App Modernization â Upgrade for .NET experience.</p>
<h3 id="try-convert"><code>try-convert</code></h3>
<p>The <code>try-convert</code> tool is a .NET global tool that can convert a project or entire solution to the .NET SDK, including moving desktop apps to .NET. However, this tool isn't recommended if your project has a complicated build process such as custom tasks, targets, or imports.</p>
<p>For more information, see the <a href="https://github.com/dotnet/try-convert" data-linktype="external"><code>try-convert</code> GitHub repository</a>.</p>
<h3 id="platform-compatibility-analyzer">Platform compatibility analyzer</h3>
<p>The <a href="../../standard/analyzers/platform-compat-analyzer" data-linktype="relative-path">Platform compatibility analyzer</a> analyzes whether or not you're using an API that throws a <a href="/en-us/dotnet/api/system.platformnotsupportedexception" class="no-loc" data-linktype="absolute-path">PlatformNotSupportedException</a> at run time. Although finding one of these APIs is unlikely if you're moving from .NET Framework 4.7.2 or higher, it's good to check. For more information about APIs that throw exceptions on .NET, see <a href="../compatibility/unsupported-apis" data-linktype="relative-path">APIs that always throw exceptions on .NET Core</a>.</p>
<p>For more information, see <a href="../../standard/analyzers/platform-compat-analyzer" data-linktype="relative-path">Platform compatibility analyzer</a>.</p>
<h2 id="considerations-when-porting">Considerations when porting</h2>
<p>When porting your application to .NET, consider the following suggestions in order:</p>
<p>âï¸ CONSIDER using the <a href="upgrade-assistant-overview" data-linktype="relative-path">.NET Upgrade Assistant</a> to migrate your projects. Even though this tool is in preview, it automates most of the manual steps detailed in this article and gives you a great starting point for continuing your migration path.</p>
<p>âï¸ CONSIDER examining your dependencies first. Your dependencies must target .NET, .NET Standard, or .NET Core.</p>
<p>âï¸ DO migrate from a NuGet <em>packages.config</em> file to <a href="/en-us/nuget/consume-packages/package-references-in-project-files" data-linktype="absolute-path">PackageReference</a> settings in the project file. Use Visual Studio to <a href="/en-us/nuget/consume-packages/migrate-packages-config-to-package-reference#migration-steps" data-linktype="absolute-path">convert the <em>package.config</em> file</a>.</p>
<p>âï¸ CONSIDER upgrading to the latest project file format even if you can't yet port your app. .NET Framework projects use an outdated project format. Even though the latest project format, known as SDK-style projects, was created for .NET Core and beyond, the format also works with .NET Framework. Having your project file in the latest format gives you a good basis for porting your app in the future.</p>
<p>âï¸ DO retarget your .NET Framework project to at least .NET Framework 4.7.2. This ensures the availability of the latest API alternatives for cases where .NET Standard doesn't support existing APIs.</p>
<p>âï¸ CONSIDER targeting .NET 8, which is a long-term support (LTS) release.</p>
<p>âï¸ DO target .NET 6+ for <strong>Windows Forms and WPF</strong> projects. .NET 6 and later versions contain many improvements for Desktop apps.</p>
<p>âï¸ CONSIDER targeting .NET Standard 2.0 if you're migrating a library that might also be used with .NET Framework projects. You can also multitarget your library, targeting both .NET Framework and .NET Standard.</p>
<p>âï¸ DO add reference to the <a href="https://www.nuget.org/packages/Microsoft.Windows.Compatibility" data-linktype="external">Microsoft.Windows.Compatibility NuGet package</a> if, after migrating, you get errors of missing APIs. A large portion of the .NET Framework API surface is available to .NET via the NuGet package.</p>
<h2 id="see-also">See also</h2>
<ul>
<li><a href="upgrade-assistant-overview" data-linktype="relative-path">Overview of the .NET Upgrade Assistant</a></li>
<li><a href="/en-us/aspnet/core/migration/proper-to-2x" data-linktype="absolute-path">ASP.NET to ASP.NET Core migration</a></li>
<li><a href="/en-us/dotnet/desktop/wpf/migration/" data-linktype="absolute-path">How to upgrade a WPF desktop app to .NET</a></li>
<li><a href="/en-us/dotnet/desktop/winforms/migration/" data-linktype="absolute-path">Migrate .NET Framework Windows Forms apps to .NET</a></li>
<li><a href="../../standard/choosing-core-framework-server" data-linktype="relative-path">.NET 5 vs. .NET Framework for server apps</a></li>
</ul>
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
			class="feedback-section position-relative margin-top-lg border border-radius padding-xxs display-none-print"
			data-bi-name="open-source-feedback-section"
			data-open-source-feedback-section
			hidden
		>
			<div class="display-flex flex-direction-column flex-direction-row-tablet">
				<div
					class="width-450-tablet padding-inline-xs padding-inline-xs-tablet padding-top-xs padding-bottom-sm padding-top-xs-tablet background-color-body-medium"
				>
					<div class="display-flex flex-direction-column">
						<div class="padding-bottom-xxs">
							<span class="icon margin-right-xxs" aria-hidden="true">
								<span class="docon docon-brand-github"></span>
							</span>
							<span class="font-weight-semibold">
								Collaborate with us on GitHub
							</span>
						</div>
						<span class="line-height-normal">
							The source for this content can be found on GitHub, where you can also create and review issues and pull requests. For more information, see <a href="https://learn.microsoft.com/contribute/content/dotnet/dotnet-contribute">our contributor guide</a>.
						</span>
					</div>
				</div>
				<div
					class="display-flex gap-xs width-full-tablet flex-direction-column padding-xs justify-content-space-evenly"
				>
					<div class="media">
						
					<div class="media-left">
						<div class="image image-36x36" hidden data-open-source-image-container>
							<img
								class="theme-display is-light"
								src="https://learn.microsoft.com/media/logos/logo_net.svg"
								aria-hidden="true"
								data-open-source-image-light
							/>
							<img
								class="theme-display is-dark is-high-contrast"
								src="https://learn.microsoft.com/media/logos/logo_net.svg"
								aria-hidden="true"
								data-open-source-image-dark
							/>
						</div>
					</div>
			  

						<div class="media-content">
							<p
								class="font-size-xl font-weight-semibold margin-bottom-xxs"
								data-open-source-product-title
							>
								.NET
							</p>
							<div class="display-flex gap-xs flex-direction-column">
								<p class="line-height-normal" data-open-source-product-description></p>
								<div class="display-flex gap-xs flex-direction-column">
									<a href="#" data-github-link>
										<span class="icon margin-right-xxs" aria-hidden="true">
											<span class="docon docon-bug"></span>
										</span>
										<span>Open a documentation issue</span>
									</a>
									<a
										href="https://aka.ms/feedback/report?space=61"
										class="display-block margin-top-auto font-size-md"
										data-feedback-product-url
									>
										<span class="icon margin-right-xxs" aria-hidden="true">
											<span class="docon docon-feedback"></span>
										</span>
										<span>Provide product feedback</span>
									</a>
								</div>
							</div>
						</div>
					</div>
				</div>
			</div>
		</section>
		<!-- end feedback section -->
	
			
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

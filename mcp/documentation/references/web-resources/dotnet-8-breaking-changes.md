﻿Downloaded from: https://learn.microsoft.com/en-us/dotnet/core/compatibility/8.0

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
			<title>Breaking changes in .NET 8 | Microsoft Learn</title>
			<meta charset="utf-8" />
			<meta name="viewport" content="width=device-width, initial-scale=1.0" />
			<meta name="color-scheme" content="light dark" />

			<meta name="description" content="Navigate to the breaking changes in .NET 8." />
			<link rel="canonical" href="https://learn.microsoft.com/en-us/dotnet/core/compatibility/8.0" /> 

			<!-- Non-customizable open graph and sharing-related metadata -->
			<meta name="twitter:card" content="summary" />
			<meta name="twitter:site" content="@MicrosoftLearn" />
			<meta property="og:type" content="website" />
			<meta property="og:image:alt" content="Breaking changes in .NET 8 | Microsoft Learn" />
			<meta property="og:image" content="https://learn.microsoft.com/dotnet/media/dotnet-logo.png" />
			<!-- Page specific open graph and sharing-related metadata -->
			<meta property="og:title" content="Breaking changes in .NET 8" />
			<meta property="og:url" content="https://learn.microsoft.com/en-us/dotnet/core/compatibility/8.0" />
			<meta property="og:description" content="Navigate to the breaking changes in .NET 8." />
			<meta name="platform_id" content="9851693b-1813-e05b-24ac-9a55390285ec" /> <meta name="scope" content=".NET" />
			<meta name="locale" content="en-us" />
			 <meta name="adobe-target" content="true" />
			<meta name="uhfHeaderId" content="MSDocsHeader-DotNet" />

			<meta name="page_type" content="conceptual" />

			<!--page specific meta tags-->
			

			<!-- custom meta tags -->
			
		<meta name="apiPlatform" content="dotnet" />
	
		<meta name="author" content="gewarren" />
	
		<meta name="breadcrumb_path" content="/dotnet/breadcrumb/toc.json" />
	
		<meta name="feedback_system" content="OpenSource" />
	
		<meta name="feedback_product_url" content="https://aka.ms/feedback/report?space=61" />
	
		<meta name="ms.author" content="gewarren" />
	
		<meta name="ms.devlang" content="dotnet" />
	
		<meta name="ms.service" content="dotnet-fundamentals" />
	
		<meta name="ms.topic" content="concept-article" />
	
		<meta name="show_latex" content="true" />
	
		<meta name="ms.date" content="2025-04-10T00:00:00Z" />
	
		<meta name="document_id" content="250b5b35-569a-c5fa-2bf8-9d951bc1bdb7" />
	
		<meta name="document_version_independent_id" content="1dc7a986-52e6-3a88-0163-4a869afc75d3" />
	
		<meta name="updated_at" content="2025-08-21T08:13:00Z" />
	
		<meta name="original_content_git_url" content="https://github.com/dotnet/docs/blob/live/docs/core/compatibility/8.0.md" />
	
		<meta name="gitcommit" content="https://github.com/dotnet/docs/blob/0ea5ae5472b84362e8b438d45e10f5c8195a109c/docs/core/compatibility/8.0.md" />
	
		<meta name="git_commit_id" content="0ea5ae5472b84362e8b438d45e10f5c8195a109c" />
	
		<meta name="site_name" content="Docs" />
	
		<meta name="depot_name" content="VS.core-docs" />
	
		<meta name="schema" content="Conceptual" />
	
		<meta name="toc_rel" content="toc.json" />
	
		<meta name="pdf_url_template" content="https://learn.microsoft.com/pdfstore/en-us/VS.core-docs/{branchName}{pdfName}" />
	
		<meta name="feedback_help_link_type" content="" />
	
		<meta name="feedback_help_link_url" content="" />
	
		<meta name="search.mshattr.devlang" content="csharp" />
	
		<meta name="word_count" content="946" />
	
		<meta name="asset_id" content="core/compatibility/8.0" />
	
		<meta name="moniker_range_name" content="" />
	
		<meta name="item_type" content="Content" />
	
		<meta name="source_path" content="docs/core/compatibility/8.0.md" />
	
		<meta name="previous_tlsh_hash" content="E96E4DF1210A5BD6A1824637BE4E3AD4945855CA7EF8ADFC8014E1CA3203BA974F7CBEED43E13DC4272762DA00871B83614163ED718C82C315045B5D6FCCA67DAB9436FF98" />
	
		<meta name="github_feedback_content_git_url" content="https://github.com/dotnet/docs/blob/main/docs/core/compatibility/8.0.md" />
	 
		<meta name="cmProducts" content="https://authoring-docs-microsoft.poolparty.biz/devrel/7696cda6-0510-47f6-8302-71bb5d2e28cf" data-source="generated" />
	
		<meta name="cmProducts" content="https://authoring-docs-microsoft.poolparty.biz/devrel/d452572f-6212-498f-9050-ca4a9e50a425" data-source="generated" />
	
		<meta name="spProducts" content="https://authoring-docs-microsoft.poolparty.biz/devrel/69c76c32-967e-4c65-b89a-74cc527db725" data-source="generated" />
	
		<meta name="spProducts" content="https://authoring-docs-microsoft.poolparty.biz/devrel/a12e40b7-59a2-4437-96e2-166ce622b864" data-source="generated" />
	

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
        "name": "gewarren",
        "url": "https://github.com/gewarren"
      },
      {
        "name": "Copilot",
        "url": "https://github.com/Copilot"
      },
      {
        "name": "CamSoper",
        "url": "https://github.com/CamSoper"
      },
      {
        "name": "richlander",
        "url": "https://github.com/richlander"
      },
      {
        "name": "BillWagner",
        "url": "https://github.com/BillWagner"
      },
      {
        "name": "baronfel",
        "url": "https://github.com/baronfel"
      },
      {
        "name": "sudo-plz",
        "url": "https://github.com/sudo-plz"
      },
      {
        "name": "Rageking8",
        "url": "https://github.com/Rageking8"
      },
      {
        "name": "BartoszKlonowski",
        "url": "https://github.com/BartoszKlonowski"
      },
      {
        "name": "Falco20019",
        "url": "https://github.com/Falco20019"
      },
      {
        "name": "sbomer",
        "url": "https://github.com/sbomer"
      },
      {
        "name": "JonDouglas",
        "url": "https://github.com/JonDouglas"
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
			
			href="https://github.com/dotnet/docs/blob/main/docs/core/compatibility/8.0.md"
			data-original_content_git_url="https://github.com/dotnet/docs/blob/live/docs/core/compatibility/8.0.md"
			data-original_content_git_url_template="{repo}/blob/{branch}/docs/core/compatibility/8.0.md"
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
	
					<div class="content"><h1 id="breaking-changes-in-net-8">Breaking changes in .NET 8</h1></div>
					
		<div
			id="article-metadata"
			class="page-metadata-container display-flex gap-xxs justify-content-space-between align-items-center flex-wrap-wrap"
		>
			
		<div class="margin-block-xxs">
			<ul class="metadata page-metadata" data-bi-name="page info" lang="en-us" dir="ltr">
				  <li class="visibility-hidden-visual-diff">
			<local-time
				format="twoDigitNumeric"
				datetime="2025-04-10T08:00:00.000Z"
				data-article-date-source="calculated"
				class="is-invisible"
			>
				2025-04-10
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
	
					<div class="content"><p>If you're migrating an app to .NET 8, the breaking changes listed here might affect you. Changes are grouped by technology area, such as ASP.NET Core or Windows Forms.</p>
<p>This article categorizes each breaking change as <em>binary incompatible</em> or <em>source incompatible</em>, or as a <em>behavioral change</em>:</p>
<ul>
<li><p><strong>Binary incompatible</strong> - When run against the new runtime or component, existing binaries may encounter a breaking change in behavior, such as failure to load or execute, and if so, require recompilation.</p>
</li>
<li><p><strong>Source incompatible</strong> - When recompiled using the new SDK or component or to target the new runtime, existing source code may require source changes to compile successfully.</p>
</li>
<li><p><strong>Behavioral change</strong> - Existing code and binaries may behave differently at run time. If the new behavior is undesirable, existing code would need to be updated and recompiled.</p>
</li>
</ul>
<h2 id="aspnet-core">ASP.NET Core</h2>
<table>
<thead>
<tr>
<th>Title</th>
<th>Type of change</th>
</tr>
</thead>
<tbody>
<tr>
<td><a href="aspnet-core/8.0/concurrencylimitermiddleware-obsolete" data-linktype="relative-path">ConcurrencyLimiterMiddleware is obsolete</a></td>
<td>Source incompatible</td>
</tr>
<tr>
<td><a href="aspnet-core/8.0/problemdetails-custom-converters" data-linktype="relative-path">Custom converters for serialization removed</a></td>
<td>Behavioral change</td>
</tr>
<tr>
<td><a href="aspnet-core/8.0/forwarded-headers-unknown-proxies" data-linktype="relative-path">Forwarded Headers Middleware ignores X-Forwarded-* headers from unknown proxies</a></td>
<td>Behavioral change</td>
</tr>
<tr>
<td><a href="aspnet-core/8.0/isystemclock-obsolete" data-linktype="relative-path">ISystemClock is obsolete</a></td>
<td>Source incompatible</td>
</tr>
<tr>
<td><a href="aspnet-core/8.0/antiforgery-checks" data-linktype="relative-path">Minimal APIs: IFormFile parameters require anti-forgery checks</a></td>
<td>Behavioral change</td>
</tr>
<tr>
<td><a href="aspnet-core/8.0/addratelimiter-requirement" data-linktype="relative-path">Rate-limiting middleware requires AddRateLimiter</a></td>
<td>Behavioral change</td>
</tr>
<tr>
<td><a href="aspnet-core/8.0/securitytoken-events" data-linktype="relative-path">Security token events return a JsonWebToken</a></td>
<td>Behavioral change</td>
</tr>
<tr>
<td><a href="aspnet-core/8.0/trimmode-full" data-linktype="relative-path">TrimMode defaults to full for Web SDK projects</a></td>
<td>Source incompatible</td>
</tr>
</tbody>
</table>
<h2 id="containers">Containers</h2>
<table>
<thead>
<tr>
<th>Title</th>
<th>Type of change</th>
</tr>
</thead>
<tbody>
<tr>
<td><a href="containers/8.0/ca-certificates-package" data-linktype="relative-path">'ca-certificates' package removed from Alpine images</a></td>
<td>Binary incompatible</td>
</tr>
<tr>
<td><a href="containers/8.0/debian-version" data-linktype="relative-path">Debian container images upgraded to Debian 12</a></td>
<td>Binary incompatible/behavioral change</td>
</tr>
<tr>
<td><a href="containers/8.0/aspnet-port" data-linktype="relative-path">Default ASP.NET Core port changed to 8080</a></td>
<td>Behavioral change</td>
</tr>
<tr>
<td><a href="containers/8.0/krb5-libs-package" data-linktype="relative-path">Kerberos package removed from Alpine and Debian images</a></td>
<td>Binary incompatible</td>
</tr>
<tr>
<td><a href="containers/8.0/libintl-package" data-linktype="relative-path">'libintl' package removed from Alpine images</a></td>
<td>Behavioral change</td>
</tr>
<tr>
<td><a href="containers/8.0/multi-platform-tags" data-linktype="relative-path">Multi-platform container tags are Linux-only</a></td>
<td>Behavioral change</td>
</tr>
<tr>
<td><a href="containers/8.0/app-user" data-linktype="relative-path">New 'app' user in Linux images</a></td>
<td>Behavioral change</td>
</tr>
</tbody>
</table>
<h2 id="core-net-libraries">Core .NET libraries</h2>
<table>
<thead>
<tr>
<th>Title</th>
<th>Type of change</th>
</tr>
</thead>
<tbody>
<tr>
<td><a href="core-libraries/8.0/activity-operation-name" data-linktype="relative-path">Activity operation name when null</a></td>
<td>Behavioral change</td>
</tr>
<tr>
<td><a href="core-libraries/8.0/anonymouspipeserverstream-dispose" data-linktype="relative-path">AnonymousPipeServerStream.Dispose behavior</a></td>
<td>Behavioral change</td>
</tr>
<tr>
<td><a href="core-libraries/8.0/obsolete-apis-with-custom-diagnostics" data-linktype="relative-path">API obsoletions with custom diagnostic IDs</a></td>
<td>Source incompatible</td>
</tr>
<tr>
<td><a href="core-libraries/8.0/file-path-backslash" data-linktype="relative-path">Backslash mapping in Unix file paths</a></td>
<td>Behavioral change</td>
</tr>
<tr>
<td><a href="core-libraries/8.0/decodefromutf8-whitespace" data-linktype="relative-path">Base64.DecodeFromUtf8 methods ignore whitespace</a></td>
<td>Behavioral change</td>
</tr>
<tr>
<td><a href="core-libraries/8.0/bool-backed-enum" data-linktype="relative-path">Boolean-backed enum type support removed</a></td>
<td>Behavioral change</td>
</tr>
<tr>
<td><a href="core-libraries/8.0/complex-format" data-linktype="relative-path">Complex.ToString format changed to <code>&lt;a; b&gt;</code></a></td>
<td>Behavioral change</td>
</tr>
<tr>
<td><a href="core-libraries/8.0/drive-current-dir-paths" data-linktype="relative-path">Drive's current directory path enumeration</a></td>
<td>Behavioral change</td>
</tr>
<tr>
<td><a href="core-libraries/8.0/enumerable-sum" data-linktype="relative-path">Enumerable.Sum throws new OverflowException for some inputs</a></td>
<td>Behavioral change</td>
</tr>
<tr>
<td><a href="core-libraries/8.0/filestream-disposed-pipe" data-linktype="relative-path">FileStream writes when pipe is closed</a></td>
<td>Behavioral change</td>
</tr>
<tr>
<td><a href="core-libraries/8.0/timezoneinfo-object" data-linktype="relative-path">FindSystemTimeZoneById doesn't return new object</a></td>
<td>Behavioral change</td>
</tr>
<tr>
<td><a href="core-libraries/8.0/getgeneration-return-value" data-linktype="relative-path">GC.GetGeneration might return Int32.MaxValue</a></td>
<td>Behavioral change</td>
</tr>
<tr>
<td><a href="core-libraries/8.0/getfolderpath-unix" data-linktype="relative-path">GetFolderPath behavior on Unix</a></td>
<td>Behavioral change</td>
</tr>
<tr>
<td><a href="core-libraries/8.0/getsystemversion" data-linktype="relative-path">GetSystemVersion no longer returns ImageRuntimeVersion</a></td>
<td>Behavioral change</td>
</tr>
<tr>
<td><a href="core-libraries/8.0/itypedescriptorcontext-props" data-linktype="relative-path">ITypeDescriptorContext nullable annotations</a></td>
<td>Source incompatible</td>
</tr>
<tr>
<td><a href="core-libraries/8.0/console-readkey-legacy" data-linktype="relative-path">Legacy Console.ReadKey removed</a></td>
<td>Behavioral change</td>
</tr>
<tr>
<td><a href="core-libraries/8.0/parameterinfo-hasdefaultvalue" data-linktype="relative-path">Method builders generate parameters with HasDefaultValue set to false</a></td>
<td>Behavioral change</td>
</tr>
<tr>
<td><a href="core-libraries/8.0/processstartinfo-windowstyle" data-linktype="relative-path">ProcessStartInfo.WindowStyle honored when UseShellExecute is false</a></td>
<td>Behavioral change</td>
</tr>
<tr>
<td><a href="core-libraries/8.0/runtimeidentifier" data-linktype="relative-path">RuntimeIdentifier returns platform for which runtime was built</a></td>
<td>Behavioral change</td>
</tr>
<tr>
<td><a href="core-libraries/8.0/type-gettype" data-linktype="relative-path"><code>Type.GetType</code> throws exception for all invalid element types</a></td>
<td>Behavioral change</td>
</tr>
</tbody>
</table>
<h2 id="cryptography">Cryptography</h2>
<table>
<thead>
<tr>
<th>Title</th>
<th>Type of change</th>
<th>Introduced</th>
</tr>
</thead>
<tbody>
<tr>
<td><a href="cryptography/8.0/aesgcm-auth-tag-size" data-linktype="relative-path">AesGcm authentication tag size on macOS</a></td>
<td>Behavioral change</td>
<td>Preview 1</td>
</tr>
<tr>
<td><a href="cryptography/8.0/rsa-encrypt-decrypt-value-obsolete" data-linktype="relative-path">RSA.EncryptValue and RSA.DecryptValue obsolete</a></td>
<td>Source incompatible</td>
<td>Preview 1</td>
</tr>
</tbody>
</table>
<h2 id="deployment">Deployment</h2>
<table>
<thead>
<tr>
<th>Title</th>
<th>Type of change</th>
</tr>
</thead>
<tbody>
<tr>
<td><a href="deployment/8.0/rid-asset-list" data-linktype="relative-path">Host determines RID-specific assets</a></td>
<td>Binary incompatible/behavioral change</td>
</tr>
<tr>
<td><a href="deployment/8.0/monitor-image" data-linktype="relative-path">.NET Monitor only includes distroless images</a></td>
<td>Behavioral change</td>
</tr>
<tr>
<td><a href="deployment/8.0/stripsymbols-default" data-linktype="relative-path">StripSymbols defaults to true</a></td>
<td>Behavioral change</td>
</tr>
</tbody>
</table>
<h2 id="entity-framework-core">Entity Framework Core</h2>
<p><a href="/en-us/ef/core/what-is-new/ef-core-8.0/breaking-changes" data-linktype="absolute-path">Breaking changes in EF Core 8</a></p>
<h2 id="extensions">Extensions</h2>
<table>
<thead>
<tr>
<th>Title</th>
<th>Type of change</th>
</tr>
</thead>
<tbody>
<tr>
<td><a href="extensions/8.0/activatorutilities-createinstance-behavior" data-linktype="relative-path">ActivatorUtilities.CreateInstance behaves consistently</a></td>
<td>Behavioral change</td>
</tr>
<tr>
<td><a href="extensions/8.0/activatorutilities-createinstance-null-provider" data-linktype="relative-path">ActivatorUtilities.CreateInstance requires non-null provider</a></td>
<td>Behavioral change</td>
</tr>
<tr>
<td><a href="extensions/8.0/configurationbinder-exceptions" data-linktype="relative-path">ConfigurationBinder throws for mismatched value</a></td>
<td>Behavioral change</td>
</tr>
<tr>
<td><a href="extensions/8.0/configurationmanager-package" data-linktype="relative-path">ConfigurationManager package no longer references System.Security.Permissions</a></td>
<td>Source incompatible</td>
</tr>
<tr>
<td><a href="extensions/8.0/directoryservices-package" data-linktype="relative-path">DirectoryServices package no longer references System.Security.Permissions</a></td>
<td>Source incompatible</td>
</tr>
<tr>
<td><a href="extensions/8.0/dictionary-configuration-binding" data-linktype="relative-path">Empty keys added to dictionary by configuration binder</a></td>
<td>Behavioral change</td>
</tr>
<tr>
<td><a href="extensions/8.0/hostapplicationbuilder-ctor" data-linktype="relative-path">HostApplicationBuilderSettings.Args respected by HostApplicationBuilder ctor</a></td>
<td>Behavioral change</td>
</tr>
<tr>
<td><a href="extensions/8.0/dmtf-todatetime" data-linktype="relative-path">ManagementDateTimeConverter.ToDateTime returns a local time</a></td>
<td>Behavioral change</td>
</tr>
<tr>
<td><a href="extensions/8.0/cbor-datetime" data-linktype="relative-path">System.Formats.Cbor DateTimeOffset formatting change</a></td>
<td>Behavioral change</td>
</tr>
</tbody>
</table>
<h2 id="globalization">Globalization</h2>
<table>
<thead>
<tr>
<th>Title</th>
<th>Type of change</th>
</tr>
</thead>
<tbody>
<tr>
<td><a href="globalization/8.0/typeconverter-cultureinfo" data-linktype="relative-path">Date and time converters honor culture argument</a></td>
<td>Behavioral change</td>
</tr>
<tr>
<td><a href="globalization/8.0/twodigityearmax-default" data-linktype="relative-path">TwoDigitYearMax default is 2049</a></td>
<td>Behavioral change</td>
</tr>
</tbody>
</table>
<h2 id="interop">Interop</h2>
<table>
<thead>
<tr>
<th>Title</th>
<th>Type of change</th>
</tr>
</thead>
<tbody>
<tr>
<td><a href="interop/8.0/comwrappers-unwrap" data-linktype="relative-path">CreateObjectFlags.Unwrap only unwraps on target instance</a></td>
<td>Behavioral change</td>
</tr>
<tr>
<td><a href="interop/8.0/marshal-modes" data-linktype="relative-path">Custom marshallers require additional members</a></td>
<td>Source incompatible</td>
</tr>
<tr>
<td><a href="interop/8.0/idispatchimplattribute-removed" data-linktype="relative-path">IDispatchImplAttribute API is removed</a></td>
<td>Binary incompatible</td>
</tr>
<tr>
<td><a href="interop/8.0/jsfunctionbinding-constructor" data-linktype="relative-path">JSFunctionBinding implicit public default constructor removed</a></td>
<td>Binary incompatible</td>
</tr>
<tr>
<td><a href="interop/8.0/safehandle-constructor" data-linktype="relative-path">SafeHandle types must have public constructor</a></td>
<td>Source incompatible</td>
</tr>
<tr>
<td><a href="interop/8.0/linux-netcoredeps" data-linktype="relative-path">Linux native library resolution no longer uses <code>netcoredeps</code></a></td>
<td>Behavioral change</td>
</tr>
</tbody>
</table>
<h2 id="networking">Networking</h2>
<table>
<thead>
<tr>
<th>Title</th>
<th>Type of change</th>
</tr>
</thead>
<tbody>
<tr>
<td><a href="networking/8.0/sendfile-connectionless" data-linktype="relative-path">SendFile throws NotSupportedException for connectionless sockets</a></td>
<td>Behavioral change</td>
</tr>
<tr>
<td><a href="networking/8.0/uri-comparison" data-linktype="relative-path">User info in <code>mailto:</code> URIs is compared</a></td>
<td>Behavioral change</td>
</tr>
</tbody>
</table>
<h2 id="reflection">Reflection</h2>
<table>
<thead>
<tr>
<th>Title</th>
<th>Type of change</th>
</tr>
</thead>
<tbody>
<tr>
<td><a href="reflection/8.0/function-pointer-reflection" data-linktype="relative-path">IntPtr no longer used for function pointer types</a></td>
<td>Behavioral change</td>
</tr>
</tbody>
</table>
<h2 id="sdk">SDK</h2>
<table>
<thead>
<tr>
<th>Title</th>
<th>Type of change</th>
</tr>
</thead>
<tbody>
<tr>
<td><a href="sdk/8.0/console-encoding" data-linktype="relative-path">CLI console output uses UTF-8</a></td>
<td>Behavioral change/Source and binary incompatible</td>
</tr>
<tr>
<td><a href="sdk/8.0/console-encoding-fix" data-linktype="relative-path">Console encoding not UTF-8 after completion</a></td>
<td>Behavioral change/Binary incompatible</td>
</tr>
<tr>
<td><a href="sdk/8.0/default-image-tag" data-linktype="relative-path">Containers default to use the 'latest' tag</a></td>
<td>Behavioral change</td>
</tr>
<tr>
<td><a href="sdk/8.0/dotnet-pack-config" data-linktype="relative-path">'dotnet pack' uses Release configuration</a></td>
<td>Behavioral change/Source incompatible</td>
</tr>
<tr>
<td><a href="sdk/8.0/dotnet-publish-config" data-linktype="relative-path">'dotnet publish' uses Release configuration</a></td>
<td>Behavioral change/Source incompatible</td>
</tr>
<tr>
<td><a href="sdk/8.0/getx-duplicate-output" data-linktype="relative-path">Duplicate output for -getItem, -getProperty, and -getTargetResult</a></td>
<td>Behavioral change</td>
</tr>
<tr>
<td><a href="sdk/8.0/implicit-global-using-netfx" data-linktype="relative-path">Implicit <code>using</code> for System.Net.Http no longer added</a></td>
<td>Behavioral change/Source incompatible</td>
</tr>
<tr>
<td><a href="sdk/8.0/custombuildeventargs" data-linktype="relative-path">MSBuild custom derived build events deprecated</a></td>
<td>Behavioral change</td>
</tr>
<tr>
<td><a href="sdk/8.0/msbuild-language" data-linktype="relative-path">MSBuild respects DOTNET_CLI_UI_LANGUAGE</a></td>
<td>Behavioral change</td>
</tr>
<tr>
<td><a href="sdk/8.0/runtimespecific-app-default" data-linktype="relative-path">Runtime-specific apps not self-contained</a></td>
<td>Source/binary incompatible</td>
</tr>
<tr>
<td><a href="sdk/8.0/arch-option" data-linktype="relative-path">--arch option doesn't imply self-contained</a></td>
<td>Behavioral change</td>
</tr>
<tr>
<td><a href="sdk/8.0/dotnet-restore-audit" data-linktype="relative-path">'dotnet restore' produces security vulnerability warnings</a></td>
<td>Behavioral change</td>
</tr>
<tr>
<td><a href="sdk/8.0/rid-graph" data-linktype="relative-path">SDK uses a smaller RID graph</a></td>
<td>Behavioral change/Source incompatible</td>
</tr>
<tr>
<td><a href="sdk/8.0/debugsymbols" data-linktype="relative-path">Setting DebugSymbols to false disables PDB generation</a></td>
<td>Behavioral change</td>
</tr>
<tr>
<td><a href="sdk/8.0/source-link" data-linktype="relative-path">Source Link included in the .NET SDK</a></td>
<td>Source incompatible</td>
</tr>
<tr>
<td><a href="sdk/8.0/trimming-unsupported-targetframework" data-linktype="relative-path">Trimming may not be used with .NET Standard or .NET Framework</a></td>
<td>Behavioral change</td>
</tr>
<tr>
<td><a href="sdk/8.0/unlisted-versions" data-linktype="relative-path">Unlisted packages not installed by default for .NET tools</a></td>
<td>Behavioral change</td>
</tr>
<tr>
<td><a href="sdk/8.0/user-file" data-linktype="relative-path">.user file imported in outer builds</a></td>
<td>Behavioral change</td>
</tr>
<tr>
<td><a href="sdk/8.0/version-requirements" data-linktype="relative-path">Version requirements for .NET 8 SDK</a></td>
<td>Source incompatible</td>
</tr>
</tbody>
</table>
<h2 id="serialization">Serialization</h2>
<table>
<thead>
<tr>
<th>Title</th>
<th>Type of change</th>
</tr>
</thead>
<tbody>
<tr>
<td><a href="serialization/8.0/binaryformatter-disabled" data-linktype="relative-path">BinaryFormatter disabled for most projects</a></td>
<td>Behavioral change</td>
</tr>
<tr>
<td><a href="serialization/8.0/publishtrimmed" data-linktype="relative-path">PublishedTrimmed projects fail reflection-based serialization</a></td>
<td>Behavioral change</td>
</tr>
<tr>
<td><a href="serialization/8.0/metadata-resolving" data-linktype="relative-path">Reflection-based deserializer resolves metadata eagerly</a></td>
<td>Behavioral change</td>
</tr>
</tbody>
</table>
<h2 id="windows-forms">Windows Forms</h2>
<table>
<thead>
<tr>
<th>Title</th>
<th>Type of change</th>
</tr>
</thead>
<tbody>
<tr>
<td><a href="windows-forms/8.0/picturebox-remote-image" data-linktype="relative-path">Certs checked before loading remote images in PictureBox</a></td>
<td>Behavioral change</td>
</tr>
<tr>
<td><a href="windows-forms/8.0/datetimepicker-text" data-linktype="relative-path">DateTimePicker.Text is empty string</a></td>
<td>Behavioral change</td>
</tr>
<tr>
<td><a href="windows-forms/8.0/defaultvalueattribute-removal" data-linktype="relative-path">DefaultValueAttribute removed from some properties</a></td>
<td>Behavioral change</td>
</tr>
<tr>
<td><a href="windows-forms/8.0/exceptioncollection" data-linktype="relative-path">ExceptionCollection ctor throws ArgumentException</a></td>
<td>Behavioral change</td>
</tr>
<tr>
<td><a href="windows-forms/8.0/top-level-window-scaling" data-linktype="relative-path">Forms scale according to AutoScaleMode</a></td>
<td>Behavioral change</td>
</tr>
<tr>
<td><a href="windows-forms/8.0/imagelist-colordepth" data-linktype="relative-path">ImageList.ColorDepth default is Depth32Bit</a></td>
<td>Behavioral change</td>
</tr>
<tr>
<td><a href="windows-forms/8.0/extensions-package-deps" data-linktype="relative-path">System.Windows.Extensions doesn't reference System.Drawing.Common</a></td>
<td>Source incompatible</td>
</tr>
<tr>
<td><a href="windows-forms/8.0/tablelayoutstylecollection" data-linktype="relative-path">TableLayoutStyleCollection throws ArgumentException</a></td>
<td>Behavioral change</td>
</tr>
<tr>
<td><a href="windows-forms/8.0/forms-scale-size-to-dpi" data-linktype="relative-path">Top-level forms scale minimum and maximum size to DPI</a></td>
<td>Behavioral change</td>
</tr>
<tr>
<td><a href="windows-forms/8.0/domainupdownaccessibleobject" data-linktype="relative-path">WFDEV002 obsoletion is now an error</a></td>
<td>Source incompatible</td>
</tr>
</tbody>
</table>
<h2 id="see-also">See also</h2>
<ul>
<li><a href="../../csharp/whats-new/breaking-changes/compiler%20breaking%20changes%20-%20dotnet%208" data-linktype="relative-path">C# compiler breaking changes in C# 12 / .NET 8</a></li>
<li><a href="../whats-new/dotnet-8/overview" data-linktype="relative-path">What's new in .NET 8</a></li>
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

using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Moq.AutoMock;
using Reacher.App;
using Reacher.App.Services;
using Reacher.Common.Logic;
using Reacher.Common.Utilities;
using Reacher.Data.External;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Reacher.Tests;
public class EmailIngestionTests : TestBase
{
    private readonly WebApplicationFactory<Program> _app;
    private readonly IServiceProvider _services;
    private readonly AutoMocker _autoMock;
    private readonly HttpClient _httpClient;

    public EmailIngestionTests()
    {
        _app = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices((ctx, services) =>
            {
                DependencyInjection.AddDependencies(services, ctx.Configuration);
            });
            // ... Configure test services
        });
        _httpClient = _app.CreateDefaultClient();
        var scope = _app.Server.Services.GetService<IServiceScopeFactory>().CreateScope();
        _services = scope.ServiceProvider;
        _autoMock = new AutoMocker();
        //var contentProvider = _autoMock.CreateInstance<TemplateService>();
        //_autoMock.Use<ITemplateService>(contentProvider);
        //_autoMock.Use<IEmailContentRenderer>(_autoMock.CreateInstance<EmailContentRenderer>());
        //_autoMock.Use<ISendGridParser>(_autoMock.CreateInstance<SendGridParser>());
        //_autoMock.Use(GetDbContext());
    }

    [Fact]
    public async Task TestProcess()
    {
        Guid invoiceId = Guid.NewGuid();
        var ingester = _services.GetService<IEmailIngestionService>();
        await ingester.IngestEmail(GetTestReachEmail());
        //await ingester.IngestEmail(GetTestResponseEmail());
    }

    private Stream GetTestResponseEmail()
    {
        var str = @"--xYzZY
Content-Disposition: form-data; name=""headers""

Received: by mx0160p1mdw1.sendgrid.net with SMTP id EmvQLZtIEZ Tue, 22 Feb 2022 04:11:32 +0000 (UTC)
Received: from mail-oo1-f45.google.com (unknown [209.85.161.45]) by mx0160p1mdw1.sendgrid.net (Postfix) with ESMTPS id 32D6036097B for <walker@reacher.me>; Tue, 22 Feb 2022 04:11:32 +0000 (UTC)
Received: by mail-oo1-f45.google.com with SMTP id i6-20020a4ac506000000b0031c5ac6c078so8064103ooq.6 for <walker@reacher.me>; Mon, 21 Feb 2022 20:11:32 -0800 (PST)
DKIM-Signature: v=1; a=rsa-sha256; c=relaxed/relaxed; d=bluesam-com.20210112.gappssmtp.com; s=20210112; h=mime-version:references:in-reply-to:from:date:message-id:subject:to; bh=UaWiUbIxLJbTYlXJ1vHur1gktDlv6Og0mboL/lZG2MI=; b=mbyCBPJNM16V2E8/Uri/YzVoBySrRa/s604HnzoZsNg6O7pRBVThjRShqOW87T532K ET/deVNHNRgfv3CbwiPzHXknu91fEGDyRGO+gZL7R1yO5fDSwTV5yiC4lK829ZiFfBVz 1e0by5AAyEL04T0VOBGME/g3uODbLCzY9dP84qMA/KZJnXdipTGwLiPF1wBqFzkLocM5 bX16bIptHhvfBLRKWNfMRaxnOvIH7mrxbXdvrCuR8aRiESRcKA/G9x3xOIg8/3ARzQoA FmOZjqPd4rROkIr3Q7dbDdRN5JOH1eohLlFIjXgcJ9DCXPEDCw4eGRdu3TKlX+/gaQyJ 9EZQ==
X-Google-DKIM-Signature: v=1; a=rsa-sha256; c=relaxed/relaxed; d=1e100.net; s=20210112; h=x-gm-message-state:mime-version:references:in-reply-to:from:date :message-id:subject:to; bh=UaWiUbIxLJbTYlXJ1vHur1gktDlv6Og0mboL/lZG2MI=; b=ncKBkpo5J2QM8Qt1C5pIhElxIIRRL25LD9/krg21JoaeHhKrKggm0MBiHtO2C0ZSnz bqpJ3QqzvcrfTadWDwZhWPA1Ee0YwOOkF7Zd/I84Sm/XpeNoB2+rqrY8yXKG70w0ulWP WAshezI4qe1X+9zK0gFyCFlDbJinc2gIMRPRUk0fN9CBnq6/AezLxeh8S+gMttPHDH5M YaE07tBeMAINSOlAk+P1dxHeFyZ+0qsLnZVzbcJdwRw8xmZB6eS62041QWLr7l3KuDO4 hG4Rbl/qqAbs6Jq2J/pf7J63eQqMlmQR6qKU6wdVaql41zfjN2Yaqqx4D3I5t0n59tXK EeHQ==
X-Gm-Message-State: AOAM5324v2mRT6nEzbP6ieafQfv7lpNQGJav9g5a4kYaFGFvmwE9LKsp OdzJJmfhS4pHR9NH7wl+QFb5F9VUvtEgWWUewkDEs8sr+LQAXw==
X-Google-Smtp-Source: ABdhPJzlxW5JTLPko/m6MPd2ZJJdyXuM+7SnJ9VAUMf8gM2dle7looKK+yB4z6ZwVjhQrjmkcRtxHOqh7sC/eKTm33I=
X-Received: by 2002:a05:6870:4610:b0:d2:4f7a:2e04 with SMTP id z16-20020a056870461000b000d24f7a2e04mr835051oao.251.1645503091268; Mon, 21 Feb 2022 20:11:31 -0800 (PST)
MIME-Version: 1.0
References: <aX4Qqrv2RNaL8-FLoSBjKw@ismtpd0047p1las1.sendgrid.net>
In-Reply-To: <aX4Qqrv2RNaL8-FLoSBjKw@ismtpd0047p1las1.sendgrid.net>
From: Sam Walker <bluesam@hotmail.com>
Date: Mon, 21 Feb 2022 21:11:20 -0700
Message-ID: <CAC74Hn+BVFOwG8BY8h26JBLhUQ1BCNHt-QKJggNXqsyRf+b6yQ@mail.gmail.com>
Subject: Re: Reacher - from bluesam@hotmail.com (Samuel Walker) | Wanna get in touch | 2a6541b2-9451-473c-a313-bb74b307ea13
To: Reacher Service <walker+2a6541b2-9451-473c-a313-bb74b307ea13@reacher.me>
Content-Type: multipart/alternative; boundary=""0000000000003b6f5605d8938b3f""

--xYzZY
Content-Disposition: form-data; name=""dkim""

{@bluesam-com.20210112.gappssmtp.com : pass}
--xYzZY
Content-Disposition: form-data; name=""to""

Reacher Service <walker+2a6541b2-9451-473c-a313-bb74b307ea13@reacher.me>
--xYzZY
Content-Disposition: form-data; name=""html""

<div dir=""ltr"">I sure hope to as well.</div><br><div class=""gmail_quote""><div dir=""ltr"" class=""gmail_attr"">On Mon, Feb 21, 2022 at 5:33 PM Reacher Service &lt;<a href=""mailto:walker@reacher.me"">walker@reacher.me</a>&gt; wrote:<br></div><blockquote class=""gmail_quote"" style=""margin:0px 0px 0px 0.8ex;border-left:1px solid rgb(204,204,204);padding-left:1ex""><div dir=""ltr"">Really hoping we can meet up at the bitcoin conference.</div>
<img src=""http://url536.reacher.me/wf/open?upn=ga8EH1AwQLVuljZq7wATtD2FBciC1NUx1BqgofzGiElkKOU5wk0eQQldaBWz3Q0Aj3R9ET1Dgg7HTTZCzq2-2BbF5DWW2zFbMxMtd1GWUkFnzZuvMPkruloUGP7rjousMpQRcAv-2FlFWlCQPnbqTwhvT-2FYoVDVjtf218tmqfbSCx7V9ld85GzKbI6jvJ5YJ-2F2XLhAt5jJ-2Fs3EGvniAa9ABRQg-3D-3D"" alt="""" width=""1"" height=""1"" border=""0"" style=""height: 1px; width: 1px; border-width: 0px; margin: 0px; padding: 0px;"">
</blockquote></div>

--xYzZY
Content-Disposition: form-data; name=""from""

Sam Walker <bluesam@hotmail.com>
--xYzZY
Content-Disposition: form-data; name=""text""

I sure hope to as well.

On Mon, Feb 21, 2022 at 5:33 PM Reacher Service <walker@reacher.me> wrote:

> Really hoping we can meet up at the bitcoin conference.
>

--xYzZY
Content-Disposition: form-data; name=""sender_ip""

209.85.161.45
--xYzZY
Content-Disposition: form-data; name=""spam_report""

Spam detection software, running on the system ""mx0160p1mdw1.sendgrid.net"",
has NOT identified this incoming email as spam.  The original
message has been attached to this so you can view it or label
similar future email.  If you have any questions, see
@@CONTACT_ADDRESS@@ for details.

Content preview:  I sure hope to as well. On Mon, Feb 21, 2022 at 5:33 PM Reacher
   Service <walker@reacher.me> wrote: > Really hoping we can meet up at the
  bitcoin conference. > [...] 

Content analysis details:   (1.6 points, 5.0 required)

 pts rule name              description
---- ---------------------- --------------------------------------------------
 1.6 HTML_IMAGE_ONLY_12     BODY: HTML: images with 800-1200 bytes of words
 0.0 HTML_MESSAGE           BODY: HTML included in message
 0.0 T_REMOTE_IMAGE         Message contains an external image


--xYzZY
Content-Disposition: form-data; name=""envelope""

{""to"":[""walker@reacher.me""],""from"":""bluesam@hotmail.com""}
--xYzZY
Content-Disposition: form-data; name=""attachments""

0
--xYzZY
Content-Disposition: form-data; name=""subject""

Re: Reacher - from bluesam@hotmail.com (Samuel Walker) | Wanna get in touch | 2a6541b2-9451-473c-a313-bb74b307ea13
--xYzZY
Content-Disposition: form-data; name=""spam_score""

1.64
--xYzZY
Content-Disposition: form-data; name=""charsets""

{""to"":""UTF-8"",""html"":""UTF-8"",""subject"":""UTF-8"",""from"":""UTF-8"",""text"":""UTF-8""}
--xYzZY
Content-Disposition: form-data; name=""SPF""

pass
--xYzZY--
";
        return GenerateStreamFromString(str);
    }

    private Stream GetTestReachEmail()
    {
        var str = @"--xYzZY
Content-Disposition: form-data; name=""headers""

Received: by mx0079p1las1.sendgrid.net with SMTP id mBAmCheUPH Fri, 01 Apr 2022 14:37:22 +0000 (UTC)
Received: from mail-oa1-f43.google.com (unknown [209.85.160.43]) by mx0079p1las1.sendgrid.net (Postfix) with ESMTPS id A217420D8D for <sam@testing.reacher.me>; Fri,  1 Apr 2022 14:37:22 +0000 (UTC)
Received: by mail-oa1-f43.google.com with SMTP id 586e51a60fabf-df02f7e2c9so2865192fac.10 for <sam@testing.reacher.me>; Fri, 01 Apr 2022 07:37:22 -0700 (PDT)
DKIM-Signature: v=1; a=rsa-sha256; c=relaxed/relaxed; d=bluesam-com.20210112.gappssmtp.com; s=20210112; h=mime-version:from:date:message-id:subject:to; bh=02EMjoj/RnszSxIk7/8eA11fNNslA6dYdBgrVFsLb0M=; b=MgDs5Y1D/Q1m6HpxuhxZcgf4A5SH6i1XDDggWyY6bZEvOqIZyLWhPXrvC4CuvIQG4b cy1dOTGKgfqcQmlcaFvPLOdvUeN90Dy4sfANMCjgBWuPDHlTI/4HN8Jji1jMLm0LQ+OI o/m8UMreDTrQsQYf5NglBiBB9zMxMcah1VGW96JR3RdQEFUUZ3eILNA5lGaZwJQjkg7c KznYFcK4zxSOtOmPGZQllLxRFbxxRFVOA1Vr/gPc8qIeJoxB8u2M7I75nxIWGkGIkuUB uLkj81MPCepKC8kSB8Aq7od/R7Nx9svYvrxw/2N16DlH36Q4BCZpA/WH/YNe5b0Qgvk1 x8vQ==
X-Google-DKIM-Signature: v=1; a=rsa-sha256; c=relaxed/relaxed; d=1e100.net; s=20210112; h=x-gm-message-state:mime-version:from:date:message-id:subject:to; bh=02EMjoj/RnszSxIk7/8eA11fNNslA6dYdBgrVFsLb0M=; b=66PyH4omBgBoyI6D3amx5x6HmWpfD5tQJYW2mG+gmoGWeKLhHaaU0870lLk9JUGcJr SQJMLZgMc7SPdBn+GsFXTea6HduPXa9zlQoTiFt1jG/roXIMY18AR6TNWmRpIQRoJ4AK huRV1HW2OiagOj/bNIht8doJYJ9eUs5TO8/sM2EL1hq9kDJ5JCb20mFUUjPlzL6+zgNE iz7sXTURbYkWu9Tla0/B8QsIwt7TnW9M7n+j7OXPSghc614idpVmhNpyee1c7ms0RX7J Kwt+wS5IBM1JOGJm+dgQ9GXB3wLsz+0S0MkBg6bkfGPZSSeI981n7V1aEzMTRNt+PZly i2eQ==
X-Gm-Message-State: AOAM533JcJctaTLj3R8gZ7rlHXjRxP/+0Ov3CD85xevc4FjsnKJxu5X9 M5imu77Y9rgaeGAtdQ/7YDiun5piL+gh2IKea63VCgbhPiPNUg==
X-Google-Smtp-Source: ABdhPJzy3fPxLTFJNn/pjznoxyF/+LTgCczerU+MnWizduMYE0+qEd5StQ8pM8r8WTr5QYU/oJp/ihrskQr/jqR0KzI=
X-Received: by 2002:a05:6870:4790:b0:dd:cd0f:e00b with SMTP id c16-20020a056870479000b000ddcd0fe00bmr5134544oaq.120.1648823841468; Fri, 01 Apr 2022 07:37:21 -0700 (PDT)
MIME-Version: 1.0
From: Sam Walker <sam@bluesam.com>
Date: Fri, 1 Apr 2022 08:37:09 -0600
Message-ID: <CAC74HnLNkPamK9A0a6UGnM1HhhHWWnWa8zL3h8+HE_mwnZ=V2Q@mail.gmail.com>
Subject: A Third Try
To: sam@testing.reacher.me
Content-Type: multipart/alternative; boundary=""0000000000005e428205db98b726""

--xYzZY
Content-Disposition: form-data; name=""dkim""

{@bluesam-com.20210112.gappssmtp.com : pass}
--xYzZY
Content-Disposition: form-data; name=""to""

sam@testing.reacher.me
--xYzZY
Content-Disposition: form-data; name=""html""

<div dir=""ltr"">Should see in beeceptor</div>

--xYzZY
Content-Disposition: form-data; name=""from""

Sam Walker <sam@bluesam.com>
--xYzZY
Content-Disposition: form-data; name=""text""

Should see in beeceptor

--xYzZY
Content-Disposition: form-data; name=""sender_ip""

209.85.160.43
--xYzZY
Content-Disposition: form-data; name=""spam_report""

Spam detection software, running on the system ""mx0079p1las1.sendgrid.net"",
has NOT identified this incoming email as spam.  The original
message has been attached to this so you can view it or label
similar future email.  If you have any questions, see
@@CONTACT_ADDRESS@@ for details.

Content preview:  Should see in beeceptor Should see in beeceptor [...] 

Content analysis details:   (0.0 points, 5.0 required)

 pts rule name              description
---- ---------------------- --------------------------------------------------
 0.0 HTML_MESSAGE           BODY: HTML included in message
 0.0 T_MIME_NO_TEXT         No (properly identified) text body parts


--xYzZY
Content-Disposition: form-data; name=""envelope""

{""to"":[""sam@testing.reacher.me""],""from"":""sam@bluesam.com""}
--xYzZY
Content-Disposition: form-data; name=""attachments""

0
--xYzZY
Content-Disposition: form-data; name=""subject""

A Third Try
--xYzZY
Content-Disposition: form-data; name=""spam_score""

0.011
--xYzZY
Content-Disposition: form-data; name=""charsets""

{""to"":""UTF-8"",""html"":""UTF-8"",""subject"":""UTF-8"",""from"":""UTF-8"",""text"":""UTF-8""}
--xYzZY
Content-Disposition: form-data; name=""SPF""

pass
--xYzZY--
";
        return GenerateStreamFromString(str);
    }
    static Stream GenerateStreamFromString(string s)
    {
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        writer.Write(s);
        writer.Flush();
        stream.Position = 0;
        return stream;
    }
}

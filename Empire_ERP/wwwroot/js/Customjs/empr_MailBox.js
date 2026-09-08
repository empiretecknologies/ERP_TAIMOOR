var empr_MailBox = {
    InitEvents: function () {
        $(document).ready(function () {
            empr_MailBox.FetchMails();
            $('body').on('click', '#sendEmailBtn', function (e) {
                e.preventDefault();
                empr_MailBox.SendMail();
            });
        });
    },

    SendMail: function () {
        debugger
        $("#Loader").show();
        $("#Loader").css('display', 'flex');
        const formData = {
            To: $('#emailTo').val(),
            Subject: $('#emailSubject').val(),
            Message: $('#emailMessage').val()
        };
        if (formData.To != '' && formData.To != null) {
            ajaxHelper.ajaxPostJsonData(formData, '/MailBox/SendMail', function (data) {
                if (data.msgType == 1) {
                    empr_helper.notify(data.data, data.msgType);
                    $('#emailTo').val('');
                    $('#emailSubject').val('');
                    $('#emailMessage').val('');
                }
                else {
                    empr_helper.notify(data.data, data.msgType);
                }
                $("#Loader").hide();
            }, false, true);
        } else {
            $("#Loader").hide();
            empr_helper.notify("Email To is required !", 2);
        }
    },

    //FetchMails: function () {
    //    $("#Loader").show();
    //    $("#Loader").css('display', 'flex');
    //    ajaxHelper.ajaxGetJson('/MailBox/GetInboxEmails?count=' + 20, function (data) {
    //        let emails = data.data.result;
    //        console.log(emails)
    //        if (data.msgType == 1) {
    //            empr_helper.notify("Fetched Successfulyy! ", data.msgType);
    //            const inbox = document.getElementById('emailInbox');
    //            emails.forEach((email, index) => {
    //                const emailElement = document.createElement('div');
    //                emailElement.className = `media ${index === 0 ? 'active' : ''}`; // First email gets 'active' class

    //                emailElement.innerHTML = `
    //                    <div class="media-body">
    //                        <h6>${empr_MailBox.getSenderName(email.from)}</h6>
    //                        <p>${email.subject}</p>
    //                        <p style="font-size: 9px;">(${empr_MailBox.formatEmailDate(email.date)})</p>
    //                    </div>
    //                `;

    //                inbox.appendChild(emailElement);
    //            });
    //        }
    //        else {
    //            empr_helper.notify(data.data, data.msgType);
    //        }
    //        $("#Loader").hide();
    //    }, false, true);
    //},

    FetchMails: function () {
        $("#Loader").show();
        $("#Loader").css('display', 'flex');
        ajaxHelper.ajaxGetJson('/MailBox/GetInboxEmails?count=' + 20, function (data) {
            let emails = data.data.result;
            console.log(emails)
            if (data.msgType == 1) {
                empr_helper.notify("Fetched Successfully! ", data.msgType);
                const inbox = document.getElementById('emailInbox');
                inbox.innerHTML = '';

                // Show only first 5 emails initially
                const initialEmails = emails.slice(0, 5);
                const remainingEmails = emails.slice(5);

                // Render initial emails
                initialEmails.forEach((email, index) => {
                    const emailElement = this.createEmailElement(email, index === 0);
                    inbox.appendChild(emailElement);
                });

                // Add "See All" button if there are more emails
                if (remainingEmails.length > 0) {
                    const seeAllButton = document.createElement('button');
                    seeAllButton.className = 'btn btn-link see-all-btn';
                    seeAllButton.innerHTML = 'See All (' + remainingEmails.length + ')';
                    seeAllButton.addEventListener('click', () => {
                        // Render remaining emails
                        remainingEmails.forEach((email) => {
                            const emailElement = this.createEmailElement(email, false);
                            inbox.insertBefore(emailElement, seeAllButton);
                        });
                        // Remove the "See All" button
                        seeAllButton.remove();
                    });
                    inbox.appendChild(seeAllButton);
                }

                // Load first email by default
                if (emails.length > 0) {
                    empr_MailBox.loadFullEmail(emails[0].id);
                }
            }
            else {
                empr_helper.notify(data.data, data.msgType);
            }
            $("#Loader").hide();
        }.bind(this), false, true); // Note the .bind(this) to maintain context
    },

    // Helper function to create email element
    createEmailElement: function (email, isActive) {
        const emailElement = document.createElement('div');
        emailElement.className = `media ${isActive ? 'active' : ''}`;
        emailElement.dataset.id = email.id;

        emailElement.innerHTML = `
        <div class="media-body">
            <h6>${empr_MailBox.getSenderName(email.from)}</h6>
            <p>${email.subject}</p>
            <p style="font-size: 9px;">(${empr_MailBox.formatEmailDate(email.date)})</p>
        </div>
    `;

        emailElement.addEventListener('click', function () {
            empr_MailBox.loadFullEmail(email.id);
            document.querySelectorAll('.media').forEach(el => {
                el.classList.remove('active');
            });
            this.classList.add('active');
            $('#pills-darkprofile-tab').click();
        });

        return emailElement;
    },

    formatEmailDate(dateString) {
        const date = new Date(dateString);
        const day = date.getDate();
        const month = date.toLocaleString('default', { month: 'long' });
        const year = date.getFullYear();
        return `${day} ${month} ${year}`;
    },

    getSenderName(fromString) {
        const match = fromString.match(/"(.*?)"/);
        return match ? match[1] : fromString.split('<')[0].trim();
    },

    loadFullEmail: function (emailId) {
        $("#Loader").show();
        $("#Loader").css('display', 'flex');

        ajaxHelper.ajaxGetJson('/MailBox/GetFullEmail?id=' + emailId, function (data) {
            console.log(data)
            if (data.msgType == 1) {
                const email = data.data.result;
                empr_MailBox.displayFullEmail(email);
            } else {
                empr_helper.notify(data.data, data.msgType);
            }
            $("#Loader").hide();
        }, false, true);
    },

    displayFullEmail: function (email) {
        const emailDetail = document.getElementById('emailDetail');
        emailDetail.innerHTML = '';
        let attachmentsHtml = '';
        if (email.attachments && email.attachments.length > 0) {
            attachmentsHtml = `
            <div class="email-attachments">
                <h6>Attachments (${email.attachments.length})</h6>
                <ul>
                    ${email.attachments.map(attachment => `
                        <li>
                            <a href="/MailBox/DownloadAttachment?emailId=${email.id}&attachmentId=${attachment.id}" 
                               target="_blank">
                                ${attachment.fileName} (${empr_MailBox.formatFileSize(attachment.size)})
                            </a>
                        </li>
                    `).join('')}
                </ul>
            </div>
        `;
        }

        emailDetail.innerHTML = `
        <div class="email-header">
            <h4>${email.subject}</h4>
            <div class="email-meta">
                <span class="sender">From: ${empr_MailBox.getSenderName(email.from)}</span>
                <span class="date">Date: ${empr_MailBox.formatEmailDate(email.date)}</span>
            </div>
        </div>
        <div class="email-body">
            ${email.body || '[No content]'}
        </div>
        ${attachmentsHtml}
    `;
    },

    formatFileSize: function (bytes) {
        if (bytes === 0) return '0 Bytes';
        const k = 1024;
        const sizes = ['Bytes', 'KB', 'MB', 'GB'];
        const i = Math.floor(Math.log(bytes) / Math.log(k));
        return parseFloat((bytes / Math.pow(k, i)).toFixed(2) + ' ' + sizes[i]);
    }
}
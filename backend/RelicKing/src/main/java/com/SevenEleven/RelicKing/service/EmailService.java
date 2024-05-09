package com.SevenEleven.RelicKing.service;

import org.springframework.mail.SimpleMailMessage;
import org.springframework.mail.javamail.JavaMailSender;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import com.SevenEleven.RelicKing.common.exception.CustomException;
import com.SevenEleven.RelicKing.common.exception.ExceptionType;

import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;

@Slf4j
@Service
@Transactional
@RequiredArgsConstructor
public class EmailService {

	private final JavaMailSender javaMailSender;

	/**
	 * 이메일을 발송하는 메서드
	 */
	public void sendEmail(String toEmail, String title, String text) {

		SimpleMailMessage emailForm = createEmailForm(toEmail, title, text);
		try {
			javaMailSender.send(emailForm);
		} catch (RuntimeException e) {
			ExceptionType exceptionType = ExceptionType.FAILED_TO_SEND_EMAIL;
			log.info(exceptionType.getMessage());
			throw new CustomException(exceptionType);
		}
	}

	/**
	 * 발송할 이메일 데이터를 설정하는 메서드
	 */
	private SimpleMailMessage createEmailForm(String toEmail, String title, String text) {

		SimpleMailMessage message = new SimpleMailMessage();
		message.setTo(toEmail);
		message.setSubject(title);
		message.setText(text);

		return message;
	}
}

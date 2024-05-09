package com.SevenEleven.RelicKing.dto.request;

import com.SevenEleven.RelicKing.common.validation.EmailGroup;
import com.SevenEleven.RelicKing.common.validation.NotBlankGroup;

import jakarta.validation.constraints.Email;
import jakarta.validation.constraints.NotBlank;
import lombok.Getter;

@Getter
public class VerifyEmailRequestDto {

	@NotBlank(message = "이메일은 필수 입력 값입니다.", groups = NotBlankGroup.class)
	@Email(message = "이메일 형식이 올바르지 않습니다.", groups = EmailGroup.class)
	private String email;

	private String code;
}

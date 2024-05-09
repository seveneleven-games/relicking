package com.SevenEleven.RelicKing.dto.request;

import com.SevenEleven.RelicKing.common.validation.NotBlankGroup;
import com.SevenEleven.RelicKing.common.validation.PatternGroup;

import jakarta.validation.constraints.NotBlank;
import jakarta.validation.constraints.Pattern;
import lombok.Getter;

@Getter
public class UpdateNicknameRequestDto {

	@NotBlank(message = "닉네임은 필수 입력 값입니다.", groups = NotBlankGroup.class)
	@Pattern(
		regexp = "^[가-힣a-zA-Z0-9!@#$%^&*()_+=-]{1,12}$",
		message = "닉네임은 한글, 영문, 숫자, 특수문자만 가능하며 12자 이내여야 합니다.",
		groups = PatternGroup.class
	)
	private String nickname;
}

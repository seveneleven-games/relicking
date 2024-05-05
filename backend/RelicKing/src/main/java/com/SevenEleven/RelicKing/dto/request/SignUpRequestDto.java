package com.SevenEleven.RelicKing.dto.request;

import com.SevenEleven.RelicKing.entity.Member;

import jakarta.validation.constraints.Email;
import jakarta.validation.constraints.NotBlank;
import jakarta.validation.constraints.Pattern;
import lombok.Getter;

@Getter
public class SignUpRequestDto {

	@NotBlank(message = "이메일은 필수 입력 값입니다.")
	@Email(message = "이메일 형식이 올바르지 않습니다.")
	private String email;

	@NotBlank(message = "닉네임은 필수 입력 값입니다.")    // FIXME : Pattern 조건에 걸린다.
	@Pattern(regexp = "^[가-힣a-zA-Z0-9!@#$%^&*()_+=-]{1,12}$",
		message = "닉네임은 한글, 영문, 숫자, 특수문자만 가능하며 12자 이내여야 합니다.")
	private String nickname;

	@NotBlank(message = "비밀번호는 필수 입력 값입니다.")    // FIXME : Pattern 조건에 걸린다.
	@Pattern(regexp = "^(?=.*[A-Za-z])(?=.*\\d)(?=.*[!@#$%^&*()_+=-])[A-Za-z\\d!@#$%^&*()_+=-]{8,16}$",
		message = "비밀번호는 영문, 숫자, 특수문자를 각각 최소 하나씩 포함하며 8~16자여야 합니다.")
	private String password;

	public Member toEntity(String encryptedPassword) {
		return Member.builder()
			.email(this.email)
			.nickname(this.nickname)
			.password(encryptedPassword)
			.build();
	}
}

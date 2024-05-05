package com.SevenEleven.RelicKing.controller;

import org.springframework.http.HttpStatus;
import org.springframework.security.core.annotation.AuthenticationPrincipal;
import org.springframework.validation.annotation.Validated;
import org.springframework.web.bind.annotation.DeleteMapping;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PatchMapping;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RequestParam;
import org.springframework.web.bind.annotation.RestController;

import com.SevenEleven.RelicKing.common.response.Response;
import com.SevenEleven.RelicKing.common.security.CustomUserDetails;
import com.SevenEleven.RelicKing.common.validation.ValidationSequence;
import com.SevenEleven.RelicKing.dto.request.ReissueRequestDto;
import com.SevenEleven.RelicKing.dto.request.SignUpRequestDto;
import com.SevenEleven.RelicKing.dto.request.UpdateNicknameRequestDto;
import com.SevenEleven.RelicKing.dto.request.UpdatePasswordRequestDto;
import com.SevenEleven.RelicKing.dto.request.VerifyEmailRequestDto;
import com.SevenEleven.RelicKing.dto.response.ReissueResponseDto;
import com.SevenEleven.RelicKing.service.MemberService;

import io.swagger.v3.oas.annotations.Operation;
import io.swagger.v3.oas.annotations.media.Content;
import io.swagger.v3.oas.annotations.media.Schema;
import io.swagger.v3.oas.annotations.responses.ApiResponse;
import io.swagger.v3.oas.annotations.tags.Tag;
import lombok.RequiredArgsConstructor;

@Tag(name = "Member", description = "사용자 관련 API")
@RestController
@RequestMapping("/api/members")
@RequiredArgsConstructor
public class MemberController {

	private final MemberService memberService;

	@Operation(
		summary = "회원 가입",
		description = "닉네임 : 영문, 숫자, 특수문자로 12자 이내 / 비밀번호 : 영문, 숫자, 특수문자 각각 하나 이상 포함하여 8~16자"
	)
	@ApiResponse(
		responseCode = "200", description = "회원 가입 완료",
		content = @Content(schema = @Schema(implementation = boolean.class))
	)
	@PostMapping("/signup")
	public Response signup(@RequestBody @Validated(ValidationSequence.class) SignUpRequestDto dto) {
		memberService.signup(dto);
		return new Response(HttpStatus.OK.value(), "회원가입이 완료되었습니다.", true);
	}

	@Operation(
		summary = "access token 재발급",
		description = "refresh token을 활용하여 access token 및 refresh token을 재발급 받습니다."
	)
	@ApiResponse(
		responseCode = "200", description = "access token 및 refresh token 재발급 성공",
		content = @Content(schema = @Schema(implementation = ReissueResponseDto.class))
	)
	@PostMapping("/reissue")
	public Response reissue(@RequestBody ReissueRequestDto dto) {
		ReissueResponseDto reissueResponseDto = memberService.reissue(dto.getRefreshToken());
		return new Response(HttpStatus.OK.value(), "access token과 refresh token이 재발급되었습니다.", reissueResponseDto);
	}

	@Operation(
		summary = "로그아웃",
		description = "사용자의 모든 refresh token을 무효화합니다."
	)
	@ApiResponse(
		responseCode = "200", description = "로그아웃 성공",
		content = @Content(schema = @Schema(implementation = boolean.class))
	)
	@DeleteMapping("/logout")
	public Response logout(@AuthenticationPrincipal CustomUserDetails customUserDetails) {
		memberService.logout(customUserDetails.getMember());
		return new Response(HttpStatus.OK.value(), "로그아웃 되었습니다.", true);
	}

	@Operation(
		summary = "닉네임 변경",
		description = "사용자의 닉네임을 변경합니다. 이미 사용 중인 닉네임으로는 변경할 수 없습니다. (영문, 숫자, 특수문자로 12자 이내)"
	)
	@ApiResponse(
		responseCode = "200", description = "닉네임 변경 성공",
		content = @Content(schema = @Schema(implementation = boolean.class))
	)
	@PatchMapping("/nickname")
	public Response updateNickname(@AuthenticationPrincipal CustomUserDetails customUserDetails, @RequestBody @Validated(ValidationSequence.class) UpdateNicknameRequestDto dto) {
		memberService.updateNickname(customUserDetails.getMember(), dto.getNickname());
		return new Response(HttpStatus.OK.value(), "닉네임이 변경되었습니다.", true);
	}

	@Operation(
		summary = "비밀번호 변경",
		description = "사용자의 비밀번호를 변경합니다. (영문, 숫자, 특수문자 각각 하나 이상 포함하여 8~16자)"
	)
	@ApiResponse(
		responseCode = "200", description = "비밀번호 변경 성공",
		content = @Content(schema = @Schema(implementation = boolean.class))
	)
	@PatchMapping("/password")
	public Response updatePassword(@AuthenticationPrincipal CustomUserDetails customUserDetails, @RequestBody @Validated(ValidationSequence.class) UpdatePasswordRequestDto dto) {
		memberService.updatePassword(customUserDetails.getMember(), dto.getPassword());
		return new Response(HttpStatus.OK.value(), "비밀번호가 변경되었습니다.", true);
	}

	@Operation(
		summary = "이메일 중복 체크",
		description = "해당 이메일이 이미 사용 중인지 검사합니다."
	)
	@ApiResponse(
		responseCode = "200", description = "이메일 사용 가능",
		content = @Content(schema = @Schema(implementation = boolean.class))
	)
	@GetMapping("/duplicate-email")
	public Response checkEmailForDuplicates(@RequestParam(value = "email") String email) {
		return memberService.checkEmailForDuplicates(email);
	}

	@Operation(
		summary = "닉네임 중복 체크",
		description = "해당 닉네임이 이미 사용 중인지 검사합니다."
	)
	@ApiResponse(
		responseCode = "200", description = "닉네임 사용 가능",
		content = @Content(schema = @Schema(implementation = boolean.class))
	)
	@GetMapping("/duplicate-nickname")
	public Response checkNicknameForDuplicates(@RequestParam(value = "nickname") String nickname) {
		return memberService.checkNicknameForDuplicates(nickname);
	}

	@Operation(
		summary = "이메일 인증 코드 발송",
		description = "이메일 인증 코드를 발송하며 서버에서 해당 코드를 일정 시간동안 저장해둡니다."
	)
	@ApiResponse(
		responseCode = "200", description = "이메일 인증 코드 발송 성공",
		content = @Content(schema = @Schema(implementation = boolean.class))
	)
	@PostMapping("/emails/code")
	public Response sendCodeToEmail(@RequestBody @Validated(ValidationSequence.class) VerifyEmailRequestDto dto) {
		memberService.sendCodeToEmail(dto.getEmail());
		return new Response(HttpStatus.OK.value(), "이메일로 인증 코드를 전송하였습니다.", true);
	}

	@Operation(
		summary = "이메일 인증",
		description = "인증 코드를 통해 이메일을 인증합니다."
	)
	@ApiResponse(
		responseCode = "200", description = "이메일 인증 성공",
		content = @Content(schema = @Schema(implementation = boolean.class))
	)
	@PostMapping("/emails/verification")
	public Response verifyEmail(@RequestBody @Validated(ValidationSequence.class) VerifyEmailRequestDto dto) {
		memberService.verifyEmail(dto.getEmail(), dto.getCode());
		return new Response(HttpStatus.OK.value(), "인증되었습니다.", true);
	}
}

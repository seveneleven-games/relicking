package com.SevenEleven.RelicKing.repository;

import java.util.Optional;

import org.springframework.data.jpa.repository.EntityGraph;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.data.jpa.repository.Query;
import org.springframework.data.repository.query.Param;

import com.SevenEleven.RelicKing.entity.Member;

public interface MemberRepository extends JpaRepository<Member, Integer> {

	@EntityGraph(attributePaths = "memberRelics")
	@Query("select m from Member m where m.memberId = :memberId")
	Optional<Member> selectOne(@Param("memberId") Integer memberId);
}
